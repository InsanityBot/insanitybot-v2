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
        public static TimeSpan ParseTimeSpan(this String value, Nullable<TemporaryPunishmentType> Type = null)
        {
            if (TimeSpan.TryParse(value, out var time))
                return time;
            switch (Type)
            {
                case TemporaryPunishmentType.Mute:
                    if (TimeSpan.TryParse((String)InsanityBot.Config["insanitybot.commands.default_mute_time"], out var muteDefault))
                        return muteDefault;
                    else
                        break;
                case TemporaryPunishmentType.Ban:
                    if (TimeSpan.TryParse((String)InsanityBot.Config["insanitybot.commands.default_ban_time"], out var banDefault))
                        return banDefault;
                    else
                        break;
                default:
                    //hardcoded fallback default in case the config fails
                    return new TimeSpan(00, 30, 00);
            }

            //even more hardcoded fallback of a fallback, this method should just never fail
            return new TimeSpan(00, 30, 00);
        }
    }
}
