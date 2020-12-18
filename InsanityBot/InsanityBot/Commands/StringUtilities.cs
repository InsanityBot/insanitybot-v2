using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Commands.Services.Converters.Time;
using InsanityBot.Utility.Exceptions;

using Microsoft.Extensions.Logging;

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
            try
            {
                return new TimeSpanParser().ConvertToTimeSpan(value);
            }
            catch
            {
                InsanityBot.Client.Logger.LogError(new EventId(0010, "TimeSpanParser"), $"Could not parse {value}");
            }            
            return type switch
            {
                TemporaryPunishmentType.Mute => TimeSpan.Parse((String)InsanityBot.Config["insanitybot.commands.default_mute_time"]),
                TemporaryPunishmentType.Ban => TimeSpan.Parse((String)InsanityBot.Config["insanitybot.commands.default_ban_time"]),
                _ => new TimeSpan(00, 30, 00)
            };
            
        }
    }
}
