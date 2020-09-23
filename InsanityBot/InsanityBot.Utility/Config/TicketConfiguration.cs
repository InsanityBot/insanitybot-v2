using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility.Config
{
    public class TicketConfiguration : IConfiguration
    {
        public String DataVersion { get; set; }
        public Dictionary<String, Object> Configuration { get; set; }

        public String TicketIdentifier { get; set; }
        public Object this[String Identifier]
        {
            get => Configuration[Identifier];
            set => Configuration[Identifier] = value;
        }
    }
}
