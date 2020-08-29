using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility.Config.Reference
{
    public class DiscordConfig
    {
        public ConfigIds Identifiers { get; set; }

        // automod settings
        public Boolean Automod { get; set; }
        public List<String> AutomodWords { get; set; }
        public Boolean AutomodLinks { get; set; }
        public List<String> WhitelistedLinks { get; set; }

        // application settings
        public List<String> ApplicationKeys { get; set; }
        public Boolean HandleApplicationQuestionsDms { get; set; }

        // ping reaction settings
        public Dictionary<UInt64, String> RolePingMessages { get; set; }
    }
}
