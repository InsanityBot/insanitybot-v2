using System;

namespace InsanityBot.Tickets.Closure
{
    public struct ClosingQueueEntry
    {
        public UInt64 ChannelId { get; set; }
        public Boolean Cancellable { get; set; }
        public DateTimeOffset CloseDate { get; set; }
    }
}
