namespace InsanityBot.Tickets;
using System;

public record TicketAccess
{
	public UInt64[] AllowedRoles { get; set; }
	public UInt64[] AllowedUsers { get; set; }
}