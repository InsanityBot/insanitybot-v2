using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Workflows
{
    public struct TicketWorkflowStep
    {
        public UInt64 CategoryId { get; set; }
        public Boolean Queued { get; set; }
    }
}
