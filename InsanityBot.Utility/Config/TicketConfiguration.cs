using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Config
{
    public class TicketConfiguration : IConfiguration<Object>
    {
        public String DataVersion { get; set; }
        public Dictionary<String, Object> Configuration { get; set; }

        public Object this[String Identifier]
        {
            get => this.Configuration[Identifier];
            set => this.Configuration[Identifier] = value;
        }

        public TicketConfiguration()
        {
            this.DataVersion = "2.0.0";
            this.Configuration = new();
        }
    }
}
