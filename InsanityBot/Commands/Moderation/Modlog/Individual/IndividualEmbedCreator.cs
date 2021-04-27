using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;

namespace InsanityBot.Commands.Moderation.Modlog.Individual
{
    public static class IndividualEmbedCreator
    {
        public static String CreateModlogDescription(this DiscordUser user, ModlogEntryType type, Boolean paged = true)
        {
            UserModlog modlog = ((DiscordMember)user).GetUserModlog();
            modlog.Modlog.Reverse(); // display newest first
            String description = "";

            if (paged)
            {
                for (Int32 b = 0; b < modlog.ModlogEntryCount; b++)
                {
                    if(modlog.Modlog[b].Type == type)
                        description += $"{modlog.Modlog[b].Type.ToString().ToUpper()}: {modlog.Modlog[b].Time:yyyy/MM/dd HH:mm:ss} - {modlog.Modlog[b].Reason}\n\n";
                }
            }
            else
            {
                for (Int32 b = 0; b < Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]); b++)
                {
                    if(modlog.Modlog[b].Type == type)
                        description += $"{modlog.Modlog[b].Type.ToString().ToUpper()}: {modlog.Modlog[b].Time:yyyy/MM/dd HH:mm:ss} - {modlog.Modlog[b].Reason}\n\n";
                }


                if (modlog.ModlogEntryCount > Convert.ToInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]))
                {
                    description += InsanityBot.LanguageConfig["insanitybot.commands.modlog.overflow"];
                }
            }

            return description;
        }
    }
}
