using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace InsanityBot.Utility.Modlogs.Reference
{
    public class ModlogUser
    {
        [XmlAttribute]
        public String Username { get; set; }

        [XmlAttribute]
        public UInt16 ModlogEntryCount { get; set; }

        [XmlAttribute]
        public Byte VerbalLogEntryCount { get; set; }


        public List<ModlogEntry> Modlog { get; set; }
        public List<VerbalModlogEntry> VerbalLog { get; set; } 
    }
}
