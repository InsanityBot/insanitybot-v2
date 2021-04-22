using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets
{
    public record TicketAccess
    {
        public UInt64[] AllowedRoles { get; set; }
        public UInt64[] AllowedUsers { get; set; }
    }
}
