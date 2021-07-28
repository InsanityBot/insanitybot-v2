using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

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
    }
}
