using CommandLine;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using InsanityBot.Commands.Miscellaneous;
using InsanityBot.Commands.Moderation;
using InsanityBot.Commands.Moderation.Locking;
using InsanityBot.Commands.Moderation.Modlog;
using InsanityBot.Commands.Permissions;
using InsanityBot.ConsoleCommands.Integrated;
using InsanityBot.Core.Logger;
using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Datafixers;
using InsanityBot.Tickets;
using InsanityBot.Tickets.Commands;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;
using InsanityBot.Utility.Timers;

using Microsoft.Extensions.Logging;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            ConfigManager = new MainConfigurationManager();
            LanguageManager = new LanguageConfigurationManager();
            LoggerManager = new LoggerConfigurationManager();

            //read config from file
            Config = ConfigManager.Deserialize("./config/main.json");

            //validate token and guild id
            #region token
            if(String.IsNullOrWhiteSpace(Config.Token))
            {
                if(!CommandLineOptions.Interactive)
                {
                    System.Console.WriteLine("Invalid Token. Please provide a valid token in .\\config\\main.json" +
                        "\nPress any key to continue...");
                    System.Console.ReadKey();
                    return;
                }

                System.Console.Write("Your config does not contain a token. To set a token now, paste your token here. " +
                    "To abort and exit InsanityBot, type \"cancel\"\nToken: ");
                String token = System.Console.ReadLine();

                if(token.ToLower().Trim() == "cancel")
                {
                    System.Console.WriteLine("Operation aborted, exiting InsanityBot.\nPress any key to continue...");
                    System.Console.ReadKey();
                    return;
                }

                Config.Token = token;
                ConfigManager.Serialize(Config, "./config/main.json");
            }

            if(Config.GuildId == 0)
            {
                if(!CommandLineOptions.Interactive)
                {
                    System.Console.WriteLine("Invalid GuildId. Please provide a valid guild ID in .\\config\\main.json" +
                        "\nPress any key to continue...");
                    System.Console.ReadKey();
                    return;
                }

                System.Console.Write("Your config does not contain a valid guild ID. To set a guild ID now, paste your guild ID here. " +
                    "To abort and exit InsanityBot, type \"cancel\"\nGuild ID: ");
                String guildId = System.Console.ReadLine();

                if(guildId.ToLower().Trim() == "cancel")
                {
                    System.Console.WriteLine("Operation aborted, exiting InsanityBot.\nPress any key to continue...");
                    System.Console.ReadKey();
                    return;
                }

                if(UInt64.TryParse(guildId, out UInt64 id))
                {
                    Config.GuildId = id;
                    ConfigManager.Serialize(Config, "./config/main.json");
                }
                else
                {
                    System.Console.WriteLine("The provided guild ID could not be parsed. Aborting and exiting InsanityBot.\n" +
                        "Press any key to continue...");
                    System.Console.ReadKey();
                    return;
                }
            }
            #endregion

            LanguageConfig = LanguageManager.Deserialize("./config/lang.json");
            LoggerConfig = LoggerManager.Deserialize("./config/logger.json");

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
                HttpTimeout = new(00, 00, 30)
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
                HomeGuild = await Client.GetGuildAsync(Convert.ToUInt64(Config.GuildId));
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
                DmHelp = (Boolean)Config["insanitybot.commands.help.send_dms"],
                IgnoreExtraArguments = true
            };

            //create and register command client
            Client.UseCommandsNext(CommandConfiguration);
            CommandsExtension = Client.GetCommandsNext();

            PaginationEmojis InteractivityPaginationEmotes = new();
            if(ToUInt64(Config["insanitybot.identifiers.interactivity.scroll_right_emote_id"]) != 0)
            {
                InteractivityPaginationEmotes.Right = HomeGuild.Emojis[ToUInt64(Config["insanitybot.identifiers.interactivity.scroll_right_emote_id"])];
            }

            if(ToUInt64(Config["insanitybot.identifiers.interactivity.scroll_left_emote_id"]) != 0)
            {
                InteractivityPaginationEmotes.Left = HomeGuild.Emojis[ToUInt64(Config["insanitybot.identifiers.interactivity.scroll_left_emote_id"])];
            }

            if(ToUInt64(Config["insanitybot.identifiers.interactivity.skip_right_emote_id"]) != 0)
            {
                InteractivityPaginationEmotes.SkipRight = HomeGuild.Emojis[ToUInt64(Config["insanitybot.identifiers.interactivity.skip_right_emote_id"])];
            }

            if(ToUInt64(Config["insanitybot.identifiers.interactivity.skip_left_emote_id"]) != 0)
            {
                InteractivityPaginationEmotes.SkipLeft = HomeGuild.Emojis[ToUInt64(Config["insanitybot.identifiers.interactivity.skip_left_emote_id"])];
            }

            if(ToUInt64(Config["insanitybot.identifiers.interactivity.stop_emote_id"]) != 0)
            {
                InteractivityPaginationEmotes.Stop = HomeGuild.Emojis[ToUInt64(Config["insanitybot.identifiers.interactivity.stop_emote_id"])];
            }

            Interactivity = Client.UseInteractivity(new()
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PaginationEmojis = InteractivityPaginationEmotes
            });

            CommandsExtension.CommandErrored += CommandsExtension_CommandErrored;

            //start timer framework
            TimeHandler.Start();

            //register commands and events
            RegisterAllCommands();
            RegisterAllEvents();

            //initialize various parts of InsanityBots framework
            InitializeAll();

            Client.Logger.LogInformation(new EventId(1000, "Main"), $"Startup successful!");

            //start offthread TCP connection
            _ = HandleTCPConnections((Int64)Config["insanitybot.tcp_port"]);

            //start offthread XP management
            // if ((Boolean)Config["insanitybot.modules.experience"])
            ; // not implemented yet

            //start integrated offthread console management - cannot disable
            _ = Task.Run(() => { IntegratedCommandHandler.Initialize(); });

            //start offthread console management
            // if ((Boolean)Config["insanitybot.modules.console"])
            ; // not implemented yet

            // load tickets
            if((Boolean)Config["insanitybot.modules.tickets"])
            {
                _ = Task.Run(() =>
                {
                    TicketDaemon = new();
                    TicketDaemon.CommandHandler.Load();

                    TicketDaemonState state = new();
                    state.RestoreDaemonState(ref TicketDaemon);

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

        private static void RegisterAllCommands()
        {
            CommandsExtension.RegisterCommands<PermissionCommand>();

            if((Boolean)Config["insanitybot.modules.miscellaneous"])
            {
                CommandsExtension.RegisterCommands<Say>();
                CommandsExtension.RegisterCommands<Embed>();
            }
            if((Boolean)Config["insanitybot.modules.moderation"])
            {
                CommandsExtension.RegisterCommands<VerbalWarn>();
                CommandsExtension.RegisterCommands<Warn>();
                CommandsExtension.RegisterCommands<Mute>();
                CommandsExtension.RegisterCommands<Blacklist>();
                CommandsExtension.RegisterCommands<Whitelist>();
                CommandsExtension.RegisterCommands<Kick>();
                CommandsExtension.RegisterCommands<Ban>();

                CommandsExtension.RegisterCommands<Modlog>();
                CommandsExtension.RegisterCommands<ExportModlog>();
                CommandsExtension.RegisterCommands<ClearModlog>();

                CommandsExtension.RegisterCommands<Purge>();
                CommandsExtension.RegisterCommands<Slowmode>();

                CommandsExtension.RegisterCommands<Lock>();
                CommandsExtension.RegisterCommands<Unlock>();
                CommandsExtension.RegisterCommands<LockHelperCommands>();
            }
            if((Boolean)Config["insanitybot.modules.tickets"])
            {
                CommandsExtension.RegisterCommands<NewTicketCommand>();
                CommandsExtension.RegisterCommands<CloseTicketCommand>();
            }
        }

        private static void RegisterAllEvents()
        {
            Utility.Timers.Timer.TimerExpiredEvent += Mute.InitializeUnmute;
            Mute.UnmuteCompletedEvent += TimeHandler.ReenableTimer;

            Utility.Timers.Timer.TimerExpiredEvent += Ban.InitializeUnban;
            Ban.UnbanCompletedEvent += TimeHandler.ReenableTimer;

            Mute.MuteStartingEvent += TimeHandler.DisableTimer;
            Ban.BanStartingEvent += TimeHandler.DisableTimer;
        }

        private static void InitializeAll()
        {
            TimeHandler.Start();
            ModlogQueue = new(
                (ModlogMessageType.Moderation, HomeGuild.GetChannel(ToUInt64(Config["insanitybot.identifiers.commands.modlog_channel_id"]))),
                (ModlogMessageType.Administration, HomeGuild.GetChannel(ToUInt64(Config["insanitybot.identifiers.commands.admin_log_channel_id"]))));
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
    }
}
