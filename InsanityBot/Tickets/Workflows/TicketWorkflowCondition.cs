using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.EventArgs;

using InsanityBot.Tickets.Workflows.Conditions;

namespace InsanityBot.Tickets.Workflows
{
    public class TicketWorkflowCondition
    {
        public Boolean ConditionMet(TicketWorkflowEvent workflowEvent, Condition condition, DiscordEventArgs eventArgs)
        {
            return workflowEvent switch
            {
                _ => false
            };
        }
    }
}
