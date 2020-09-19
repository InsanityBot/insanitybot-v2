using System;
using System.Linq;
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
                MessageCacheSize = 2048,
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

            //initialize config
            CommandConfiguration = new CommandsNextConfiguration
            {
                CaseSensitive = false,
                StringPrefixes = Config.MainPrefix
                    .Concat(Config.AdminPrefix)
                    .ToList()
            };
            Client.UseCommandsNext(CommandConfiguration);

            //get command framework extension
            CommandsExtension = Client.GetCommandsNext();

            //register all top-level command classes
            RegisterAllCommands();

            //register all client events to their respective methods
            RegisterAllEvents();

            //change help command formatting
            FormatHelpCommand();

            //handle TCP Connections for services like HetrixTools
            HandleTCPConnections();

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

        private static void HandleTCPConnections()
        {
            throw new NotImplementedException();
        }
    }
}
