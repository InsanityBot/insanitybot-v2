using System;

namespace InsanityBot.Utility.Config.Reference
{
    public class CommandToggles
    {
        // moderation
        public Boolean Verbalwarn { get; set; }
        public Boolean VerbalLog { get; set; }
        public Boolean Warn { get; set; }
        public Boolean Unwarn { get; set; }
        public Boolean Modlog { get; set; }
        public Boolean Mute { get; set; }
        public Boolean Tempmute { get; set; }
        public Boolean Unmute { get; set; }
        public Boolean Blacklist { get; set; }
        public Boolean Whitelist { get; set; }
        public Boolean Kick { get; set; }
        public Boolean Ban { get; set; }
        public Boolean Tempban { get; set; }
        public Boolean Unban { get; set; }
        public Boolean Appeal { get; set; }
        public Boolean Slowmode { get; set; }

        // misc commands
        public Boolean Tag { get; set; }
        public Boolean Faq { get; set; }
        public Boolean Suggest { get; set; }
        public Boolean SuggestionAccept { get; set; }
        public Boolean SuggestionDeny { get; set; }
        
        // ticket commands
        public Boolean TicketNew { get; set; }
        public Boolean TicketReport { get; set; }
        public Boolean TicketAdd { get; set; }
        public Boolean TicketRemove { get; set; }
        public Boolean TicketLeave { get; set; }
        public Boolean TicketClose { get; set; }
        public Boolean TicketApply { get; set; }
        public Boolean TicketApplyAccept { get; set; }
        public Boolean TicketApplyDeny { get; set; }

        // admin commands; require admin prefix
        public Boolean Lock { get; set; }
        public Boolean Unlock { get; set; }
        public Boolean Archive { get; set; }
        public Boolean Config { get; set; }
        public Boolean Permission { get; set; }
    }
}