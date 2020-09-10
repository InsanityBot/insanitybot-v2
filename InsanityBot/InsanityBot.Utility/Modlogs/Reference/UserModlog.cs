using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace InsanityBot.Utility.Modlogs.Reference
{
    public class UserModlog
    {
        public String Username { get; set; }

        public UInt16 ModlogEntryCount { get; set; }

        public Byte VerbalLogEntryCount { get; set; }


        public List<ModlogEntry> Modlog { get; set; }
        public List<VerbalModlogEntry> VerbalLog { get; set; } 
    }
}
