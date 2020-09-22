using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

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
            Config = MainConfigManager.DeserializeMainConfiguration();

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

            //set home guild to speed up bot performance later
            HomeGuild = await Client.GetGuildAsync(Config.DiscordConfig.Identifiers.GuildId);
            _ = InitializeDefaultObjects();
            Client.Logger.LogInformation("Initializing default channels and roles...");

            //initialize config
            CommandConfiguration = new CommandsNextConfiguration
            {
                CaseSensitive = false,
                StringPrefixes = Config.MainPrefix
                    .Concat(Config.AdminPrefix)
                    .ToList()
            };
            Client.UseCommandsNext(CommandConfiguration);
            Client.Logger.LogInformation("Initializing command handler...");

            //get command framework extension
            CommandsExtension = Client.GetCommandsNext();

            //register all top-level command classes
            RegisterAllCommands();
            Client.Logger.LogInformation("Registering commands...");

            //register all client events to their respective methods
            RegisterAllEvents();
            Client.Logger.LogInformation("Registering events...");

            //change help command formatting
            FormatHelpCommand();
            Client.Logger.LogInformation("Registering Help command...");

            //handle TCP Connections for services like HetrixTools
            _ = HandleTCPConnections(Config.Port);
            Client.Logger.LogInformation("Starting TCP Listener...");

            //initialization finished, abort main thread, who needs it anyway
            await Task.Delay(-1);
        }

        private static void RegisterAllCommands()
        {
            throw new NotImplementedException();
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
