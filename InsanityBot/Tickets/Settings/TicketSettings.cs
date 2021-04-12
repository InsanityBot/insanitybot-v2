using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Settings
{
    public record TicketSettings
    {
        public Boolean MetricFirstResponse { get; set; }
        public Boolean MetricResponseTime { get; set; }
        public Boolean MetricResolutionTime { get; set; }
        public Boolean MetricMessageCount { get; set; }
        public Boolean MetricMessagePerResolution { get; set; }

        public Boolean MentionStaffOnResponse { get; set; }
        public Boolean MentionClientOnResponse { get; set; }
        public Boolean MentionStaffRoleOnResponse { get; set; }
        public Boolean MentionAssignedStaffOnResponse { get; set; }
        public Boolean MentionAllClientsOnResponse { get; set; }

        public Boolean AutoAssignStaff { get; set; }
        public Boolean AllowAddUsers { get; set; }
    }
}