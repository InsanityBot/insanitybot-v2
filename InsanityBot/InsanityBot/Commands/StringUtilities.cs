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

        public static String GetFormattedString(String value, CommandContext context, UInt64 discordMemberId)
        {
            return value.Replace("{ID}", discordMemberId.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, DiscordMember commandTarget)
        {
            return value.Replace("{MENTION}", commandTarget.Mention)
                   .Replace("{USERNAME}", commandTarget.Username)
                   .Replace("{NICKNAME}", commandTarget.Nickname)
                   .Replace("{ID}", commandTarget.Id.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetReason(String value, String reason)
        {
            return value.Replace("{REASON}", reason);
        }

        /// <summary>
        /// This method will fall back to 00:30:00 if both input value and the config-defined defaults fail to parse
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan ParseTimeSpan(this String value, Nullable<TemporaryPunishmentType> type = null)
        {
            try
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
            catch
            {
                InsanityBot.Client.Logger.LogError(new EventId(9980, "TimeSpanParser"), $"Could not parse \"{value}\" as TimeSpan");
                return new TimeSpan(00, 30, 00);
            }
        }
    }
}
