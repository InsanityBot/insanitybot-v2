using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public static class ModlogEmbedCreator
    {
        // only pass page if paged is true as well
        public static String CreateModlogDescription(this DiscordUser user, Boolean paged = true, Byte page = 0)
        {
            UserModlog modlog = ((DiscordMember)user).GetUserModlog();
            String description = "";

            Int32 startIndex = Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]) * page,
                endIndex = Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]) + startIndex;

            Boolean fillsEmbed = modlog.ModlogEntryCount < endIndex;

            if (fillsEmbed)
                endIndex = (Int32)modlog.ModlogEntryCount;

            for (Int32 b = startIndex; b < endIndex; b++)
                description += $"{modlog.Modlog[b].Type.ToString().ToUpper()}: {modlog.Modlog[b].Time} - {modlog.Modlog[b].Reason}\n";

            if (fillsEmbed)
                return description;

            if (paged)
            {
                description += $"\n{InsanityBot.LanguageConfig["insanitybot.commands.modlog.paged.page_number"]}"
                    .Replace("{PAGE}", (page + 1).ToString())
                    .Replace("{PAGE_TOTAL}",
                        Convert.ToInt32((modlog.ModlogEntryCount /
                            Convert.ToInt32(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"])) + 1
                    ).ToString());
            }
            else
            {
                description += $"\n{InsanityBot.LanguageConfig["insanitybot.commands.modlog.overflow"]}";
            }

            return description;
        }

        public static String CreateVerballogDescription(this DiscordUser user, Boolean paged = true, Byte page = 0)
        {
            UserModlog modlog = ((DiscordMember)user).GetUserModlog();
            String description = "";

            Int32 startIndex = Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]) * page,
                endIndex = Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]) + startIndex;


            Boolean fillsEmbed = modlog.VerbalLogEntryCount < endIndex;

            if (fillsEmbed)
                endIndex = (Int32)modlog.VerbalLogEntryCount;

            for (Int32 b = startIndex; b < endIndex; b++)
                description += $"{modlog.VerbalLog[b].Time} - {modlog.VerbalLog[b].Reason}\n";

            if (fillsEmbed)
                return description;

            if (paged)
            {
                description += $"\n{InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.paged.page_number"]}"
                    .Replace("{PAGE}", (page + 1).ToString())
                    .Replace("{PAGE_TOTAL}",
                        Convert.ToInt32((modlog.ModlogEntryCount /
                            Convert.ToInt32(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"])) + 1
                    ).ToString());
            }
            else
            {
                description += $"\n{InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.overflow"]}";
            }

            return description;
        }
    }
}
