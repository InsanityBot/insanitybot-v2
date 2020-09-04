using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility.Config.Reference
{
    public class ConfigIds
    {
        // general
        public UInt64 GuildId { get; set; }

        // logging
        public UInt64 ModLogChannelId { get; set; }
        public UInt64 TicketLogChannelId { get; set; }
        public UInt64 ApplicationLogChannelId { get; set; }
        public UInt64 JoinLeaveLogChannelId { get; set; }

        // suggestions
        public UInt64 SuggestionChannelId { get; set; }
        public UInt64 DeniedSuggestionChannelId { get; set; }
        public UInt64 AcceptedSuggestionChannelId { get; set; }
        public UInt64 SuggestionUpvoteEmoteId { get; set; }
        public UInt64 SuggestionDownvoteEmoteId { get; set; }
        public UInt64 SuggestionAcceptEmoteId { get; set; }
        public UInt64 SuggestionDenyEmoteId { get; set; }

        // tickets
        public UInt64 TicketReactionMessageId { get; set; }
        public UInt64 TicketReactionEmoteId { get; set; }
        public UInt64 TicketCategoryId { get; set; }
    }
}
