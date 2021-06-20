using System;
using System.Collections.Generic;

namespace InsanityBot.Commands.Moderation.Locking
{
	public struct ChannelData
	{
		public List<UInt64> WhitelistedRoles { get; set; }
		public List<UInt64> LockedRoles { get; set; }

		public static ChannelData CreateNew()
		{
			return new ChannelData
			{
				WhitelistedRoles = new List<UInt64>(),
				LockedRoles = new List<UInt64>()
			};
		}
	}
}
