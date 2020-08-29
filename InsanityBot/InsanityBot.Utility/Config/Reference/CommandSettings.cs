using System;

namespace InsanityBot.Utility.Config.Reference
{
    public class CommandSettings
    {
        // moderation
        public Byte MinorWarnsEqualFullWarn { get; set; }

        // misc
        public Boolean AllowCommunitySuggestionAccept { get; set; }
        public Byte CommunitySuggestionAcceptThreshold { get; set; }
        public Boolean AllowCommunitySuggestionDenial { get; set; }
        public Byte CommunitySuggestionDenialThreshold { get; set; }
    }
}