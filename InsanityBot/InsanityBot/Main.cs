using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

using InsanityBot.Commands.Miscellaneous;
using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static async Task Main(String[] args)
        {
            //load command line arguments
            CommandLine.InitializeCommandLine();
            CommandLine.InsanityBotApplication.Execute(args);

            //reset if reset flag is set
            if (CommandLine.HardResetOnStartup.HasValue())
                await HardReset();

            //initialize if init flag is set
            if (CommandLine.InitializeOnStartup.HasValue())
                await Initialize();

            //load main config
            ConfigManager = new MainConfigurationManager();

            //deserialize main config
            if (!File.Exists("./config/main.json"))
            {
                if (!Directory.Exists("./config"))
                    Directory.CreateDirectory("./config");
                File.Create("./config/main.json").Close();
                await CreateMainConfig();
                Console.WriteLine("Please fill out the configuration file with your preferred values. Token and GuildId are required. " +
                    "The file is located at .\\config\\main.json");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            Config = ConfigManager.Deserialize("./config/main.json");

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

            //load command configuration
            CommandConfiguration = new CommandsNextConfiguration
            {
                CaseSensitive = false,
                StringPrefixes = (List<String>)Config["insanitybot.commands.prefixes"],
                DmHelp = (Boolean)Config["insanitybot.commands.help.send_dms"],
                IgnoreExtraArguments = true
            };

            //create and register command client
            Client.UseCommandsNext(CommandConfiguration);
            CommandsExtension = Client.GetCommandsNext();

            //register commands and events
            RegisterAllCommands();
            RegisterAllEvents();

            //register default help format
            FormatHelpCommand();

            //start offthread TCP connection
            _ = HandleTCPConnections((Int32)Config["insanitybot.tcp_port"]);

#pragma warning disable CS0642
            //start offthread XP management
            if ((Boolean)Config["insanitybot.modules.experience"])
                ; // not implemented yet

            //start offthread console management
            if ((Boolean)Config["insanitybot.console.enable"])
                ; // not implemented yet

            //abort main thread, who needs it anyway
            Thread.Sleep(-1);
        }

        private static void RegisterAllCommands()
        {
            if((Boolean)Config["insanitybot.module.miscellaneous"])
            {
                CommandsExtension.RegisterCommands<Say>();
            }
        }

        private static void RegisterAllEvents()
        {
            throw new NotImplementedException();
        }

        private static void FormatHelpCommand()
        {
            throw new NotImplementedException();
        }

        private static async Task HandleTCPConnections(Int32 Port)
        {
            if (Port == -1)
                return;


            TcpListener listener = new TcpListener(IPAddress.Parse("0.0.0.0"), Port);

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
