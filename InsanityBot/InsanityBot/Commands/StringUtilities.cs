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

using Microsoft.Extensions.Logging;

using TimeSpanParserUtil;

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
        public static TimeSpan ParseTimeSpan(this String value, Nullable<TemporaryPunishmentType> type = null)
        {
            if (TimeSpanParser.TryParse(value, out var time))
                return time;
            else
                return type switch
                {
                    TemporaryPunishmentType.Mute => TimeSpan.Parse((String)InsanityBot.Config["insanitybot.commands.default_mute_time"]),
                    TemporaryPunishmentType.Ban => TimeSpan.Parse((String)InsanityBot.Config["insanitybot.commands.default_ban_time"]),
                    _ => new TimeSpan(00, 30, 00)
                };
        }
    }
}
