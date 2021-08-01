using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.Commands;

using System;

namespace InsanityBot.Tickets.Commands
{
    public static class LangPlaceholders
    {
        public static String ReplaceValues(this String value, CommandContext context, DiscordChannel ticket)
        {
            return StringUtilities.GetFormattedString(value, context)
                .Replace("{TICKETCHANNEL}", ticket.Mention)
                .Replace("{TICKETCHANNELID}", ticket.Id.ToString())
                .Replace("{TICKETCHANNELNAME}", ticket.Name);
        }

        public static String ReplaceValues(this String value, CommandContext context, DiscordChannel ticket, TimeSpan time)
        {
            return StringUtilities.GetFormattedString(value, context)
                .Replace("{TICKETCHANNEL}", ticket.Mention)
                .Replace("{TICKETCHANNELID}", ticket.Id.ToString())
                .Replace("{TICKETCHANNELNAME}", ticket.Name)
                .Replace("{TICKETCLOSETIME}", time.ToString());
        }

        public static String ReplaceValues(this String value, DiscordClient client, MessageCreateEventArgs ticketMessage, DateTime closeTime)
        {
            return value.Replace("{MENTION}", ticketMessage.Author.Mention)
                .Replace("{USERNAME}", ticketMessage.Author.Username)
                .Replace("{NICKNAME}", (ticketMessage.Author as DiscordMember).Nickname)
                .Replace("{ID}", ticketMessage.Author.Id.ToString())
                .Replace("{CHANNEL}", ticketMessage.Channel.Mention)
                .Replace("{CHANNELNAME}", ticketMessage.Channel.Name)
                .Replace("{CHANNELID}", ticketMessage.Channel.Id.ToString())
                .Replace("{TICKETCLOSETIME}", closeTime.ToString());
        }
    }
}
