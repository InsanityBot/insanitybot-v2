using System;

namespace InsanityBot.Tickets
{
	public struct DiscordTicketData
	{
		public UInt64? AssignedStaff { get; set; }
		public UInt64 LatestMessage { get; set; }
		public Object[] AdditionalData { get; set; }
	}
}
