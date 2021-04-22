using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

using InsanityBot.Core.Logger;
using InsanityBot.Tickets.Daemon;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

#pragma warning disable CA2211
namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static CommandLineOptions CommandLineOptions;

        public static MainConfigurationManager ConfigManager;
        public static MainConfiguration Config;

        public static DiscordConfiguration ClientConfiguration;
        public static DiscordClient Client;

        public static DiscordGuild HomeGuild;

        public static CommandsNextExtension CommandsExtension;
        public static CommandsNextConfiguration CommandConfiguration;

        public static InteractivityExtension Interactivity;

        public static LanguageConfigurationManager LanguageManager;
        public static LanguageConfiguration LanguageConfig;

        public static LoggerConfiguration LoggerConfig;
        public static LoggerConfigurationManager LoggerManager;

        public static PermissionEngine PermissionEngine;

        public static String Version = "2.0.0-dev.00027";
        public static TicketDaemon TicketDaemon;
    }
}
#pragma warning restore CA2211
