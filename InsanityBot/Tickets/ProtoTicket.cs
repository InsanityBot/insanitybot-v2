namespace InsanityBot.Tickets;
using System;

internal struct ProtoTicket
{
	public UInt64 Creator { get; set; }
	public TicketSettings Settings { get; set; }
	public Guid TicketGuid { get; set; }
	public UInt64 DiscordChannelId { get; set; }
}