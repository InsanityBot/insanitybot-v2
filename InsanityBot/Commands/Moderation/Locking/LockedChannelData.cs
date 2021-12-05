namespace InsanityBot.Commands.Moderation.Locking;
using System;
using System.Collections.Generic;

using DSharpPlus.Entities;

public struct LockedChannelData
{
	public List<DiscordOverwrite> Overwrites { get; set; }
	public UInt64 ChannelId { get; set; }
}