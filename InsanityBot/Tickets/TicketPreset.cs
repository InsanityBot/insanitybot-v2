using DSharpPlus.Entities;

using System;

namespace InsanityBot.Tickets
{
    public record TicketPreset
    {
        public UInt64 Category { get; set; }
        public String NameFormat { get; set; }
        public String Topic { get; set; }
        public TicketAccess AccessRules { get; set; }
        public TicketSettings Settings { get; set; }
        public DiscordMessage[] CreationMessages { get; set; }
        public DiscordEmbed[] CreationEmbeds { get; set; }
    }
}
