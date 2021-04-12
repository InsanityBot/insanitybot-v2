using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Tickets.Naming;
using InsanityBot.Tickets.Settings;
using InsanityBot.Tickets.Workflows;

namespace InsanityBot.Tickets.Presets
{
    public record TicketPreset
    {
        public TicketSettings Settings { get; set; }
        public String DefaultTopic { get; set; }
        public TicketWorkflow Workflow { get; set; }
        public TicketAccess Access { get; set; }
        public TicketNaming Naming { get; set; }
    }
}
