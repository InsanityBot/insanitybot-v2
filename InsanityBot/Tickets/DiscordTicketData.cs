namespace InsanityBot.Tickets;
using System;

public struct DiscordTicketData
{
	public UInt64? AssignedStaff { get; set; }
	public UInt64 LatestMessage { get; set; }
	public Object[] AdditionalData { get; set; }
}