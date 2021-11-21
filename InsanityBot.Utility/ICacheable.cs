namespace InsanityBot.Utility;
using System;

public interface ICacheable
{
	public Guid CacheEntryGuid { get; set; }
	public DateTimeOffset LastUsedAt { get; set; }
}