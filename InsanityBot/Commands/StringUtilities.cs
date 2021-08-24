using System;
using System.Runtime.CompilerServices;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Microsoft.Extensions.Logging;

using TimeSpanParserUtil;

namespace InsanityBot.Commands
{
    public static class StringUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, DiscordMember commandTarget) => value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{USERNAME}", commandTarget.Username)
                .Replace("{NICKNAME}", commandTarget.Nickname)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, DiscordUser commandTarget) => value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{USERNAME}", commandTarget.Username)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, DiscordRole commandTarget) => value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{NAME}", commandTarget.Name)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, DiscordMember commandTarget, DiscordRole role)
            => value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{USERNAME}", commandTarget.Username)
                .Replace("{NICKNAME}", commandTarget.Nickname)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name)
                .Replace("{ROLE}", role.Mention)
                .Replace("{ROLENAME}", role.Name)
                .Replace("{ROLEID}", role.Id.ToString());


        public static String GetFormattedString(String value, CommandContext context, DiscordRole commandTarget, DiscordRole role)
            => value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{NAME}", commandTarget.Name)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name)
                .Replace("{ROLE}", role.Mention)
                .Replace("{ROLENAME}", role.Name)
                .Replace("{ROLEID}", role.Id.ToString());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, DiscordMember commandTarget, String permission)
            => value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{USERNAME}", commandTarget.Username)
                .Replace("{NICKNAME}", commandTarget.Nickname)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name)
                .Replace("{PERMISSION}", permission);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, DiscordRole commandTarget, String permission)
                => value.Replace("{MENTION}", commandTarget.Mention)
                .Replace("{NAME}", commandTarget.Name)
                .Replace("{ID}", commandTarget.Id.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name)
                .Replace("{PERMISSION}", permission);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, DiscordRole role, DiscordChannel channel)
            => value.Replace("{ROLE}", role.Mention)
                .Replace("{ROLENAME}", role.Name)
                .Replace("{ROLEID}", role.Id.ToString())
                .Replace("{CHANNEL}", channel.Mention)
                .Replace("{CHANNELNAME}", channel.Name)
                .Replace("{CHANNELID}", channel.Id.ToString());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context)
            => value.Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, CommandContext context, UInt64 discordMemberId)
            => value.Replace("{ID}", discordMemberId.ToString())
                .Replace("{MODMENTION}", context.Member.Mention)
                .Replace("{MODUSERNAME}", context.Member.Username)
                .Replace("{MODNICKNAME}", context.Member.Nickname)
                .Replace("{MODID}", context.Member.Id.ToString())
                .Replace("{CHANNEL}", context.Channel.Mention)
                .Replace("{CHANNELNAME}", context.Channel.Name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetFormattedString(String value, DiscordMember commandTarget)
            => value.Replace("{MENTION}", commandTarget.Mention)
                   .Replace("{USERNAME}", commandTarget.Username)
                   .Replace("{NICKNAME}", commandTarget.Nickname)
                   .Replace("{ID}", commandTarget.Id.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String GetReason(String value, String reason) => value.Replace("{REASON}", reason);

        public static String GetMemberReason(String value, String reason, DiscordMember member)
            => value.Replace("{REASON}", reason)
                .Replace("{MENTION}", member.Mention)
                .Replace("{NICKNAME}", member.Nickname)
                .Replace("{USERNAME}", member.Username)
                .Replace("{ID}", member.Id.ToString());

        /// <summary>
        /// This method will fall back to 00:30:00 if both input value and the config-defined defaults fail to parse
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan ParseTimeSpan(this String value, TemporaryPunishmentType? type = null)
        {
            try
            {
                if(Int32.TryParse(value, out Int32 seconds))
                {
                    return new TimeSpan(0, 0, seconds);
                }

                if(TimeSpanParser.TryParse(value, out TimeSpan time))
                {
                    return time;
                }
                else
                {
                    return type switch
                    {
                        TemporaryPunishmentType.Mute => TimeSpan.Parse(InsanityBot.Config.Value<String>("insanitybot.commands.default_mute_time")),
                        TemporaryPunishmentType.Ban => TimeSpan.Parse(InsanityBot.Config.Value<String>("insanitybot.commands.default_ban_time")),
                        _ => new TimeSpan(00, 30, 00)
                    };
                }
            }
            catch
            {
                InsanityBot.Client.Logger.LogError(new EventId(9980, "TimeSpanParser"), $"Could not parse \"{value}\" as TimeSpan");
                return new TimeSpan(00, 30, 00);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan ParseTimeSpan(this String value)
        {
            try
            {
                if(Int32.TryParse(value, out Int32 seconds))
                {
                    return new TimeSpan(0, 0, seconds);
                }

                if(TimeSpanParser.TryParse(value, out TimeSpan time))
                {
                    return time;
                }
                else
                {
                    return new TimeSpan(00, 30, 00);
                }
            }
            catch
            {
                InsanityBot.Client.Logger.LogError(new EventId(9980, "TimeSpanParser"), $"Could not parse \"{value}\" as TimeSpan");
                return new TimeSpan(00, 30, 00);
            }
        }
    }
}
