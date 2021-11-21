namespace InsanityBot.Tickets.Closure;
using System;

public struct ClosingQueueEntry
{
	public UInt64 ChannelId { get; set; }
	public Boolean Cancellable { get; set; }
	public DateTimeOffset CloseDate { get; set; }
}