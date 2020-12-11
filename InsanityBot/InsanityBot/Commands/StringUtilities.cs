using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Exceptions;

namespace InsanityBot.Commands
{
    public static class StringUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, DiscordMember commandTarget)
        {
            return value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{USERNAME}", commandTarget.Username)
                .Replace("{NICKNAME}", commandTarget.Nickname)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetReason(String value, String reason)
        {
            return value.Replace("{REASON}", reason);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan ParseTimeSpan(this String value)
        {
            if (TimeSpan.TryParse(value, out var result))
                if(result.TotalDays <= 7)
                    return result;
                else
                    throw new DurationTooLongException("Temp-Mutes and Temp-Bans cannot exceed seven days");

            //this will return the default of 00:00:00 if the conversion fails
            TimeSpan.TryParse((String)InsanityBot.Config["insanitybot.commands.default_mute_time"], out TimeSpan defaultValue);
            return defaultValue;
        }
    }
}
