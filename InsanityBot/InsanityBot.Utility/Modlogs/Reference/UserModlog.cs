using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Modlogs.Reference
{
    public class UserModlog
    {
        public String Username { get; set; }

        public UInt16 ModlogEntryCount { get; set; }

        public Byte VerbalLogEntryCount { get; set; }


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
