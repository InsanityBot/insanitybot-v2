using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Tickets.Scripts;

namespace InsanityBot.Tickets.Workflows
{
    public struct TicketWorkflowTransition
    {
        public TicketWorkflowStep Origin { get; set; }
        public TicketWorkflowStep Target { get; set; }

        public TicketWorkflowEvent TriggerSinks { get; set; }
        public TicketWorkflowCondition Condition { get; set; }
        public TicketScript Transition { get; set; }
    }
}
