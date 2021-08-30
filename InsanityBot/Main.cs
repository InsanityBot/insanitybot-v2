using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using InsanityBot.ConsoleCommands.Integrated;
using InsanityBot.Core.Logger;
using InsanityBot.Datafixers;
using InsanityBot.Tickets;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;
using InsanityBot.Utility.Timers;

using Microsoft.Extensions.Logging;

using static System.Convert;

#if !DEBUG
using DSharpPlus.CommandsNext.Exceptions;
#endif

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static async Task Main(String[] args)
        {
            Console.Title = $"InsanityBot v{Version}";

            //run command line parser
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o =>
                {
                    CommandLineOptions = o;
                });


            // initialize datafixers
#if DEBUG
            DatafixerLogger.MinimalLevel = Helium.Commons.Logging.LogLevel.Debug;
#else
			DatafixerLogger.MinimalLevel = Helium.Commons.Logging.LogLevel.Warning;
#endif

            DataFixerLower.Initialize(0); //this can be switched out for 1 if you need to, insanitybot default is 0
            RegisterDatafixers();

            //load main config
            ConfigManager = new ConfigurationManager();

            //read config from file
            Config = ConfigManager.Deserialize<MainConfiguration>("./config/main.json");

            //validate token and guild id
            #region token
            if(String.IsNullOrWhiteSpace(Config.Token))
            {
                if(!CommandLineOptions.Interactive)
                {
                    Console.WriteLine("Invalid Token. Please provide a valid token in .\\config\\main.json" +
                        "\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Your config does not contain a token. To set a token now, paste your token here. " +
                    "To abort and exit InsanityBot, type \"cancel\"\nToken: ");
                String token = Console.ReadLine();

                if(token.ToLower().Trim() == "cancel")
                {
                    Console.WriteLine("Operation aborted, exiting InsanityBot.\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }

                Config.Token = token;
                ConfigManager.Serialize(Config, "./config/main.json");
            }

            if(Config.GuildId == 0)
            {
                if(!CommandLineOptions.Interactive)
                {
                    Console.WriteLine("Invalid GuildId. Please provide a valid guild ID in .\\config\\main.json" +
                        "\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Your config does not contain a valid guild ID. To set a guild ID now, paste your guild ID here. " +
                    "To abort and exit InsanityBot, type \"cancel\"\nGuild ID: ");
                String guildId = Console.ReadLine();

                if(guildId.ToLower().Trim() == "cancel")
                {
                    Console.WriteLine("Operation aborted, exiting InsanityBot.\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }

                if(UInt64.TryParse(guildId, out UInt64 id))
                {
                    Config.GuildId = id;
                    ConfigManager.Serialize(Config, "./config/main.json");
                }
                else
                {
                    Console.WriteLine("The provided guild ID could not be parsed. Aborting and exiting InsanityBot.\n" +
                        "Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }
            #endregion

            LanguageConfig = ConfigManager.Deserialize<LanguageConfiguration>("./config/lang.json");
            LoggerConfig = ConfigManager.Deserialize<LoggerConfiguration>("./config/logger.json");

            LoggerFactory loggerFactory = new();
            EmbedFactory = new();


            //create discord config; increase the cache size if you want though itll take more RAM
            ClientConfiguration = new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = Config.Token,
                TokenType = TokenType.Bot,
                MessageCacheSize = 4096,
                LoggerFactory = loggerFactory,
                HttpTimeout = new(00, 00, 30),
                Intents = DiscordIntents.All
            };

            //create and connect client
            Client = new DiscordClient(ClientConfiguration);
            await Client.ConnectAsync();

            Client.Logger.LogInformation(new EventId(1000, "Main"), $"InsanityBot Version {Version}");

            //load perms
            PermissionEngine = Client.InitializeEngine(new PermissionConfiguration
            {
                PrecompiledScripts = true,
                UpdateRolePermissions = true,
                UpdateUserPermissions = true
            });

            try
            {
                //create home guild so commands can use it
                HomeGuild = await Client.GetGuildAsync(ToUInt64(Config.GuildId));
            }
            catch(UnauthorizedException)
            {
                Client.Logger.LogCritical(new EventId(0000, "Main"),
                    "Your GuildId is either invalid or InsanityBot has not been invited to the server yet.");
            }
            catch
            {
                throw;
            }

            //load command configuration
            CommandConfiguration = new CommandsNextConfiguration
            {
                CaseSensitive = false,
                StringPrefixes = Config.Prefixes,
                DmHelp = Config.Value<Boolean>("insanitybot.commands.help.send_dms"),
                IgnoreExtraArguments = true
            };

            //create and register command client
            Client.UseCommandsNext(CommandConfiguration);
            CommandsExtension = Client.GetCommandsNext();

            Interactivity = Client.UseInteractivity(new()
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PaginationDeletion = PaginationDeletion.DeleteEmojis
            });

            CommandsExtension.CommandErrored += CommandsExtension_CommandErrored;

            //start timer framework
            TimeHandler.Start();

            //register commands and events
            RegisterAllCommands();
            RegisterAllEvents();

            //initialize various parts of InsanityBots framework
            TimeHandler.Start();

            Embeds = new();
            Embeds.Initialize(Client.Logger);

            MessageLogger = new(CommandsExtension, Client.Logger, Config, Client, HomeGuild, Embeds);

            // startup success!
            Client.Logger.LogInformation(new EventId(1000, "Main"), $"Startup successful!");

            //start offthread TCP connection
            _ = HandleTCPConnections(Config.Value<Int64>("insanitybot.tcp_port"));

            //start offthread XP management
            // if ((Boolean)Config["insanitybot.modules.experience"])
            ; // not implemented yet

            //start integrated offthread console management - cannot disable
            _ = Task.Run(() => { IntegratedCommandHandler.Initialize(); });

            //start offthread console management
            // if ((Boolean)Config["insanitybot.modules.console"])
            ; // not implemented yet

            // load tickets
            if(Config.Value<Boolean>("insanitybot.modules.tickets"))
            {
                _ = Task.Run(() =>
                {
                    TicketDaemon = new();
                    TicketDaemon.CommandHandler.Load();

                    TicketDaemonState state = new();
                    state.RestoreDaemonState(ref _ticketDaemon);

                    Client.MessageCreated += TicketDaemon.RouteCustomCommand;
                    Client.MessageCreated += TicketDaemon.ClosingQueue.HandleCancellation;
                });
            }

            //abort main thread, who needs it anyway
            Thread.Sleep(-1);
        }

        private static Task CommandsExtension_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
#if !DEBUG
            if(e.Exception.GetType() == typeof(CommandNotFoundException))
            {
                return Task.CompletedTask;
            }

            if(e.Exception.GetType() == typeof(ArgumentException))
            {
                return Task.CompletedTask;
            }

            if(e.Exception.GetType() == typeof(ArgumentNullException))
            {
                return Task.CompletedTask;
            }
#endif

            Client.Logger.LogError(new EventId(1001, "CommandError"), $"{e.Command} failed:\n" +
                $"{e.Exception}: {e.Exception.Message}\n{e.Exception.StackTrace}");
            return Task.CompletedTask;
        }

        private static async Task HandleTCPConnections(Int64 Port)
        {
            if(Port == 0)
            {
                return;
            }

            TcpListener listener = new(IPAddress.Parse("0.0.0.0"), (Int32)Port);

            try
            {
                listener.Start();

                Byte[] bytes = new Byte[256];
                TcpClient client = null;
                NetworkStream stream = null;
                Int32 i = 0;

                while(true)
                {
                    client = null;
                    stream = null;

                    client = await listener.AcceptTcpClientAsync();
                    stream = client.GetStream();

                    while((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        bytes = Encoding.ASCII.GetBytes("200");

                        stream.Write(bytes, 0, bytes.Length);
                    }

                    client.Close();
                }
            }
            catch(SocketException e)
            {
                Client.Logger.LogCritical(e.Message);
            }
            finally
            {
                listener.Stop();
            }
        }

        public static void Shutdown()
        {
            TicketDaemon.SaveAll();
            TicketDaemon.CommandHandler.Save();

            Client.DisconnectAsync();

            TicketDaemonState state = new();
            state.SaveDaemonState(TicketDaemon);

            Environment.Exit(0);
        }

        internal static void UnloadAll()
        {
            TicketDaemon.SaveAll();
            TicketDaemon.CommandHandler.Save();

            SaveLogger();

            Client.DisconnectAsync();

            TicketDaemonState state = new();
            state.SaveDaemonState(TicketDaemon);

        }
    }
}
