using System;

using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public static class ModlogEmbedCreator
    {
        public static String CreateModlogDescription(this DiscordUser user, Boolean paged = true)
        {
            UserModlog modlog = ((DiscordMember)user).GetUserModlog();
            modlog.Modlog.Reverse(); // display newest first
            String description = "";

            if (paged)
            {
                for (Int32 b = 0; b < modlog.ModlogEntryCount; b++)
                {
                    description += $"{modlog.Modlog[b].Type.ToString().ToUpper()}: {modlog.Modlog[b].Time:yyyy/MM/dd HH:mm:ss} - {modlog.Modlog[b].Reason}\n\n";
                }
            }
            else
            {
                for (Int32 b = 0; b < Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]); b++)
                {
                    description += $"{modlog.Modlog[b].Type.ToString().ToUpper()}: {modlog.Modlog[b].Time:yyyy/MM/dd HH:mm:ss} - {modlog.Modlog[b].Reason}\n\n";
                }

                if (modlog.ModlogEntryCount > Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]))
                {
                    description += InsanityBot.LanguageConfig["insanitybot.commands.modlog.overflow"];
                }
            }

            return description;
        }

        public static String CreateVerballogDescription(this DiscordUser user, Boolean paged = true, Byte page = 0)
        {
            UserModlog modlog = ((DiscordMember)user).GetUserModlog();
            modlog.VerbalLog.Reverse(); // display newest first
            String description = "";

            if (paged)
            {
                for (Int32 b = 0; b < modlog.VerbalLogEntryCount; b++)
                {
                    description += $"{modlog.VerbalLog[b].Time:yyyy/MM/dd HH:mm:ss} - {modlog.VerbalLog[b].Reason}\n\n";
                }
            }
            else
            {
                for (Int32 b = 0; b < Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]); b++)
                {
                    description += $"{modlog.VerbalLog[b].Time:yyyy/MM/dd HH:mm:ss} - {modlog.VerbalLog[b].Reason}\n\n";
                }

                if (modlog.VerbalLogEntryCount > Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]))
                {
                    description += InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.overflow"];
                }
            }

            return description;
        }
    }
}
