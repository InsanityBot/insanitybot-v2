using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility;
using InsanityBot.Utility.Reference;

namespace InsanityBot.Tickets.Daemon.Config
{
    class TicketConfiguration : IConfiguration<Object>
    {
        public String DataVersion { get; set; }
        public Dictionary<String, Object> Configuration { get; set; }

        public Object this[String Identifier]
        {
            get => Configuration[Identifier];
            set => Configuration[Identifier] = value;
        }

        public TicketConfiguration()
        {
            DataVersion = "2.0.0";
            Configuration = new();
        }
    }
}
