using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Workflows.Conditions
{
    public record Condition
    {
        public TicketWorkflowEvent Sink { get; set; }
        public String ConditionName { get; set; }
    }
}
