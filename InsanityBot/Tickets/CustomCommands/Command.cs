namespace InsanityBot.Tickets.CustomCommands;
using System;

public record Command
{
	public String Trigger { get; init; }
	public InternalCommand InternalCommand { get; init; }
	public Object Parameter { get; init; }
}