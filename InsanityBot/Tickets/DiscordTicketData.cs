using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.Tickets
{
    public struct DiscordTicketData
    {
        public UInt64? AssignedStaff { get; set; }
        public UInt64 LatestMessage { get; set; }
        public Object[] AdditionalData { get; set; }
    }
}
