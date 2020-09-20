using System;
using System.Collections.Generic;
using System.Text;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Config;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static MainConfig Config;

        public static DiscordConfiguration ClientConfiguration;
        public static DiscordClient Client;

        public static DiscordGuild HomeGuild;

        public static CommandsNextExtension CommandsExtension;
        public static CommandsNextConfiguration CommandConfiguration;

        private static void InitializeDefaultObjects()
        {

        }
    }
}
