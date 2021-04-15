using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Workflows
{
    [Flags]
    public enum TicketWorkflowEvent
    {
        MessageSent = 1,
        ReactionAdded = 2,
        CommandExecuted = 4,
        TimeElapsed = 8,
        Dummy = 16
    }
}
