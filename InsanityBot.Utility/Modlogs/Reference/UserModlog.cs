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
            Username = UserName;
            ModlogEntryCount = 0;
            VerbalLogEntryCount = 0;
            Modlog = new List<ModlogEntry>();
            VerbalLog = new List<VerbalModlogEntry>();
        }
    }
}
