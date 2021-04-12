using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Tickets.Settings;
using InsanityBot.Tickets.Workflows;

namespace InsanityBot.Tickets
{
    public class Ticket
    {
        public TicketSettings Settings { get; set; }
        public Guid TicketGuid { get; set; }
        public UInt64 DiscordChannelId { get; set; }

        public String Topic { get; set; }
        public UInt64 Owner { get; set; }
        public UInt64[] Users { get; set; }
        
        public TicketWorkflow Workflow { get; set; }
    }
}
