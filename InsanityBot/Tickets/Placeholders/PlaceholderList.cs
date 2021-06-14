using System;
using System.Collections.Generic;

namespace InsanityBot.Tickets.Placeholders
{
    internal static class PlaceholderList
    {
        public static List<String> Placeholders = new()
        {
            "user.username",
            "user.nickname",
            "user.id",
            "user.discriminator",
            "user.mention",
            "guild.name",
            "guild.id",
            "global.number",
            "global.guid",
            "global.random",
            "ticket.name",
            "ticket.mention"
        };
    }
}
