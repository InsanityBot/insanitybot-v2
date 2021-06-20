using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Modlogs.Reference
{
	public class UserModlog
	{
		public String Username { get; set; }

		public UInt32 ModlogEntryCount { get; set; }

		public UInt32 VerbalLogEntryCount { get; set; }


		public List<ModlogEntry> Modlog { get; set; }
		public List<VerbalModlogEntry> VerbalLog { get; set; }

		[JsonConstructor]
		public UserModlog() { }

		public UserModlog(String UserName)
		{
			this.Username = UserName;
			this.ModlogEntryCount = 0;
			this.VerbalLogEntryCount = 0;
			this.Modlog = new List<ModlogEntry>();
			this.VerbalLog = new List<VerbalModlogEntry>();
		}
	}
}
