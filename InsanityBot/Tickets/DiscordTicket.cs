namespace InsanityBot.Tickets;
using System;
using System.Collections.Generic;

public struct DiscordTicket
{
	public UInt64 Creator { get; set; }
	public List<UInt64> AddedUsers { get; set; }
	public IEnumerable<UInt64> Staff { get; set; }

	public String Topic { get; set; }
	public TicketSettings Settings { get; set; }

	public Guid TicketGuid { get; set; }
	public UInt64 DiscordChannelId { get; set; }
}