namespace InsanityBot.Commands.Moderation.Locking;
using System;
using System.Collections.Generic;

public struct ChannelData
{
	public List<UInt64> WhitelistedRoles { get; set; }
	public List<UInt64> LockedRoles { get; set; }

	public static ChannelData CreateNew() => new()
	{
		WhitelistedRoles = new List<UInt64>(),
		LockedRoles = new List<UInt64>()
	};
}