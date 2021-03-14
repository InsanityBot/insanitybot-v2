using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Exceptions;

using InsanityBot.Commands.Miscellaneous;
using InsanityBot.Commands.Moderation;
using InsanityBot.Commands.Moderation.Locking;
using InsanityBot.Commands.Moderation.Modlog;
using InsanityBot.Datafixers;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;
using InsanityBot.Utility.Timers;

using Microsoft.Extensions.Logging;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static async Task Main(String[] args)
        {
            //run command line parser
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(o =>
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

            //read config from file
            Config = ConfigManager.Deserialize("./config/main.json");

            if (String.IsNullOrWhiteSpace(Config.Token))
            {
                Console.WriteLine("Invalid Token. Please provide a valid token in .\\config\\main.json" +
                    "\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            LanguageConfig = LanguageManager.Deserialize("./config/lang.json");


            //create discord config; increase the cache size if you want though itll take more RAM
            ClientConfiguration = new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = Config.Token,
                TokenType = TokenType.Bot,
                MessageCacheSize = 4096,
#if DEBUG
                MinimumLogLevel = LogLevel.Debug
#else
                MinimumLogLevel = LogLevel.Information
#endif
            };

            //create and connect client
            Client = new DiscordClient(ClientConfiguration);
            await Client.ConnectAsync();

            //load perms :b
            // Client.InitializePermissionFramework();

            try
            {
                //create home guild so commands can use it
                HomeGuild = await Client.GetGuildAsync(Convert.ToUInt64(Config.GuildId));
            }
#pragma warning disable CS0168
            catch (UnauthorizedException e)
#pragma warning restore CS0168
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

            CommandsExtension.CommandErrored += CommandsExtension_CommandErrored;

            //start timer framework
            TimeHandler.Start();

            //register commands and events
            RegisterAllCommands();
            RegisterAllEvents();

            //initialize various parts of InsanityBots framework
            InitializeAll();

            Client.Logger.LogInformation(new EventId(1000, "Main"), "Startup successful!");

            //start offthread TCP connection
            _ = HandleTCPConnections((Int64)Config["insanitybot.tcp_port"]);

            //start offthread XP management
            if ((Boolean)Config["insanitybot.modules.experience"])
                ; // not implemented yet

            //start offthread console management
            if ((Boolean)Config["insanitybot.modules.console"])
                ; // not implemented yet

            //abort main thread, who needs it anyway
            Thread.Sleep(-1);
        }

        private static Task CommandsExtension_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            Client.Logger.LogError($"Command {e.Command} failed:\n" +
                $"{e.Exception}: {e.Exception.Message}\n{e.Exception.StackTrace}");
            return Task.CompletedTask;
        }

        private static void RegisterAllCommands()
        {
            if((Boolean)Config["insanitybot.modules.miscellaneous"])
            {
                CommandsExtension.RegisterCommands<Say>();
            }
            if((Boolean)Config["insanitybot.modules.moderation"])
            {
                CommandsExtension.RegisterCommands<VerbalWarn>();
                CommandsExtension.RegisterCommands<Warn>();
                CommandsExtension.RegisterCommands<Mute>();
                CommandsExtension.RegisterCommands<Blacklist>();
                CommandsExtension.RegisterCommands<Kick>();
                CommandsExtension.RegisterCommands<Ban>();

                CommandsExtension.RegisterCommands<Modlog>();
                CommandsExtension.RegisterCommands<ExportModlog>();
                CommandsExtension.RegisterCommands<ClearModlog>();

                CommandsExtension.RegisterCommands<Purge>();

                CommandsExtension.RegisterCommands<Lock>();
                CommandsExtension.RegisterCommands<Unlock>();
                CommandsExtension.RegisterCommands<LockHelperCommands>();
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

            Client.MessageReactionAdded += Modlog.ReactionAddedEventHandler;
        }

        private static void InitializeAll()
        {
            TimeHandler.Start();
        }

        private static async Task HandleTCPConnections(Int64 Port)
        {
            if (Port == 0)
                return;


            TcpListener listener = new(IPAddress.Parse("0.0.0.0"), (Int32)Port);

            try
            {
                listener.Start();

                Byte[] bytes = new Byte[256];
                TcpClient client = null;
                NetworkStream stream = null;
                Int32 i = 0;

                while (true)
                {
                    client = null;
                    stream = null;

                    client = await listener.AcceptTcpClientAsync();
                    stream = client.GetStream();

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        bytes = Encoding.ASCII.GetBytes("200");

                        stream.Write(bytes, 0, bytes.Length);
                    }

                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Client.Logger.LogCritical(e.Message);
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
