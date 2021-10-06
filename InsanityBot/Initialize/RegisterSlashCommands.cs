﻿using System;

using InsanityBot.SlashCommands.Moderation;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        private static void RegisterSlashCommands()
        {
            if(!Config.Value<Boolean>("insanitybot.modules.slashcommands"))
            {
                return;
            }

            if(Config.Value<Boolean>("insanitybot.modules.moderation"))
            {
                SlashCommandsExtension.RegisterCommands<MuteSlashCommand>(Config.GuildId);
                SlashCommandsExtension.RegisterCommands<KickSlashCommand>(Config.GuildId);
            }
        }
    }
}