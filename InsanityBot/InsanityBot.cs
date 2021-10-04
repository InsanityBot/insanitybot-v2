﻿using System;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;

using InsanityBot.Core.Formatters.Embeds;
using InsanityBot.MessageServices.Embeds;
using InsanityBot.MessageServices.Messages;
using InsanityBot.Tickets;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Language;
using InsanityBot.Utility.Permissions;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        internal static event Action SaveLogger;

        internal static TicketDaemon _ticketDaemon;

        public static CommandLineOptions CommandLineOptions { get; set; }

        public static ConfigurationManager ConfigManager { get; set; }
        public static MainConfiguration Config { get; set; }

        public static DiscordConfiguration ClientConfiguration { get; set; }
        public static DiscordClient Client { get; set; }

        public static DiscordGuild HomeGuild { get; set; }

        public static CommandsNextExtension CommandsExtension { get; set; }
        public static CommandsNextConfiguration CommandConfiguration { get; set; }

        public static SlashCommandsExtension SlashCommandsExtension { get; set; }

        public static InteractivityExtension Interactivity { get; set; }

        public static LanguageConfiguration LanguageConfig { get; set; }

        public static LoggerConfiguration LoggerConfig { get; set; }

        public static PermissionEngine PermissionEngine { get; set; }
        public static EmbedFormatterFactory EmbedFactory { get; set; }
        public static TicketDaemon TicketDaemon
        {
            get => _ticketDaemon;
            set => _ticketDaemon = value;
        }
        public static EmbedHandler Embeds { get; set; }
        public static LoggerEngine MessageLogger { get; set; }

        public static String Version => "2.0.0-dev.00043";
    }
}