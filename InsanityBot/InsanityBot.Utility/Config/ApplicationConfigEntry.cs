using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace InsanityBot.Utility.Config.Reference
{
    public class ApplicationConfigEntry
    {
        [XmlAttribute]
        public String Identifier { get; set; } // should be lowercase for consistency
        [XmlAttribute]
        public Boolean Enabled { get; set; } // that way people can disable applications without having to delete them from the files

        public List<String> Questions { get; set; } // caps out at ten questions
        public List<Nullable<UInt64>> RequiredDiscordRoles { get; set; } // caps out at ten roles
        public Nullable<DateTimeOffset> RequiredTime { get; set; } // minimal required time on the server
    }
}
