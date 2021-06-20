using System;

namespace InsanityBot.Utility
{
	public interface ICacheable
	{
		public Guid CacheEntryGuid { get; set; }
		public DateTime LastUsedAt { get; set; }
	}
}
