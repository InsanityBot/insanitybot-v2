using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions.Serialization.Reference
{
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public class Permissions
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        // moderation
        [JsonProperty("verbal_warn", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean VerbalWarn { get; set; } = false;

        [JsonProperty("verbal_log", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean VerbalLog { get; set; } = true;

        [JsonProperty("warn", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Warn { get; set; } = false;

        [JsonProperty("unwarn", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Unwarn { get; set; } = false;

        [JsonProperty("modlog", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Modlog { get; set; } = true;

        [JsonProperty("mute", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Mute { get; set; } = false;

        [JsonProperty("tempmute", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Tempmute { get; set; } = false;

        [JsonProperty("unmute", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Unmute { get; set; } = false;

        [JsonProperty("blacklist", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Blacklist { get; set; } = false;

        [JsonProperty("whitelist", NullValueHandling = NullValueHandling.Ignore )]
        public Boolean Whitelist { get; set; } = false;

        [JsonProperty("kick", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Kick { get; set; } = false;

        [JsonProperty("ban", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Ban { get; set; } = false;

        [JsonProperty("tempban", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Tempban { get; set; } = false;

        [JsonProperty("unban", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Unban { get; set; } = false;

        [JsonProperty("appeal", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Appeal { get; set; } = true;

        [JsonProperty("slowmode", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Slowmode { get; set; } = false;

        // misc commands
        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Tag { get; set; } = true;

        [JsonProperty("faq", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Faq { get; set; } = true;

        [JsonProperty("suggest", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Suggest { get; set; } = true;

        [JsonProperty("suggestion_accept", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean SuggestionAccept { get; set; } = false;

        [JsonProperty("suggestion_deny", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean SuggestionDeny { get; set; } = false;

        // ticket commands
        [JsonProperty("ticket_create", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketNew { get; set; } = true;

        [JsonProperty("ticket_report", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketReport { get; set; } = true;

        [JsonProperty("ticket_add", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketAdd { get; set; } = true;

        [JsonProperty("ticket_remove", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketRemove { get; set; } = true;

        [JsonProperty("ticket_leave", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketLeave { get; set; } = true;

        [JsonProperty("ticket_close", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketClose { get; set; } = true;

        [JsonProperty("ticket_apply", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketApply { get; set; } = true;

        [JsonProperty("ticket_apply_accept", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketApplyAccept { get; set; } = false;

        [JsonProperty("ticket_apply_deny", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean TicketApplyDeny { get; set; } = false;

        // admin commands; require admin prefix
        [JsonProperty("lock", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Lock { get; set; } = false;

        [JsonProperty("unlock", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Unlock { get; set; } = false;

        [JsonProperty("archive", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Archive { get; set; } = false;

        [JsonProperty("config", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Config { get; set; } = false;

        [JsonProperty("permission", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean Permission { get; set; } = false;
    }
}
