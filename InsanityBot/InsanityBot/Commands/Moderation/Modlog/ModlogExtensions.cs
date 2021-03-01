using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public static class ModlogExtensions
    {
        public static List<ModlogEntry> GetModlogEntries(this DiscordUser user, Byte page = 0)
        {
            return user.GetModlogEntries(Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]), 
                page);
        }

        public static List<ModlogEntry> GetModlogEntries(this DiscordUser user, UInt16 count, Byte page = 0)
        {
            UserModlog modlog = ((DiscordMember)user).GetUserModlog();

            if (modlog.ModlogEntryCount >= page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]) + count)
                return modlog.Modlog.GetRange(
                    page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]),
                    count);
            else
                return modlog.Modlog.GetRange(page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]),
                    Convert.ToInt32(modlog.ModlogEntryCount - (page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]))));
        }

        public static List<VerbalModlogEntry> GetVerballogEntries(this DiscordUser user, Byte page = 0)
        {
            return user.GetVerballogEntries(Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]),
                page);
        }

        public static List<VerbalModlogEntry> GetVerballogEntries(this DiscordUser user, UInt16 count, Byte page = 0)
        {
            UserModlog modlog = ((DiscordMember)user).GetUserModlog();

            if (modlog.VerbalLogEntryCount >= page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]) + count)
                return modlog.VerbalLog.GetRange(
                    page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]),
                    count);
            else
                return modlog.VerbalLog.GetRange(page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]),
                    Convert.ToInt32(modlog.VerbalLogEntryCount - (page * Convert.ToUInt16(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]))));
        }

        public static String ConvertToString(this ModlogEntry entry)
        { 
            return $"{entry.Type.ToString().ToUpper()}: {entry.Time} - {entry.Reason}\n";
        }

        public static String ConvertToString(this IEnumerable<ModlogEntry> entries)
        {
            String returnValue = "";
            foreach(var v in entries)
                returnValue += v.ConvertToString();
            return returnValue;
        }

        public static String ConvertToString(this VerbalModlogEntry entry)
        {
            return $"{entry.Time} - {entry.Reason}\n";
        }

        public static String ConvertToString(this IEnumerable<VerbalModlogEntry> entries)
        {
            String returnValue = "";
            foreach (var v in entries)
                returnValue += v.ConvertToString();
            return returnValue;
        }
    }
}
