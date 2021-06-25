using System;

namespace InsanityBot.Tickets
{
    public struct DiscordTicket
    {
        public UInt64 Creator { get; set; }
        public UInt64[] AddedUsers { get; set; }
        public UInt64[] Staff { get; set; }

        public String Topic { get; set; }
        public TicketSettings Settings { get; set; }

        public Guid TicketGuid { get; set; }
        public UInt64 DiscordChannelId { get; set; }
    }
}
