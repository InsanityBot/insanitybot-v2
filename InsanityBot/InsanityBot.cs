using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

using InsanityBot.Core.Formatters.Embeds;
using InsanityBot.Core.Logger;
using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;

using System;

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
        public static EmbedFormatterFactory EmbedFactory;
        public static ModlogMessageQueue ModlogQueue;

        public static String Version = "2.0.0-dev.00034";
    }
}
#pragma warning restore CA2211
