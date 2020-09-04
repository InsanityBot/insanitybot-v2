using System;
using System.Collections.Generic;
using System.Text;

using InsanityBot.Utility.Config.Reference;

namespace InsanityBot.Utility.Config
{
    public class TicketConfig
    {
        
        public Nullable<UInt64> SupportRole { get; set; } // role id that has access to the ticket no matter what
        public Nullable<UInt64> ReportRole { get; set; } // role id that has access to report tickets
        public Boolean SingleTicketMode { get; set; } // if enabled, users can only have one open ticket at a time
        public TicketNamingConvention NamingConvention { get; set; } // defines how to name tickets


        // will dm a ticket transcript to all users and send it in the ticket log channel.
        // needs more drive space when enabled. it might also increase loading and shutdown times.
        public Boolean SendTicketTranscript { get; set; } 
    }
}
