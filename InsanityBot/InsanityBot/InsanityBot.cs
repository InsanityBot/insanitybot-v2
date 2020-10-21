using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static MainConfigurationManager ConfigManager;
        public static MainConfiguration Config;

        public static DiscordConfiguration ClientConfiguration;
        public static DiscordClient Client;

        public static DiscordGuild HomeGuild;

        public static CommandsNextExtension CommandsExtension;
        public static CommandsNextConfiguration CommandConfiguration;

        public static LanguageConfigurationManager LanguageManager;
        public static LanguageConfiguration LanguageConfig;
    }
}
