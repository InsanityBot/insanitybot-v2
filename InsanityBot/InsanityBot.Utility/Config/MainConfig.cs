using System;
using System.Collections.Generic;

using InsanityBot.Utility.Config.Reference;

namespace InsanityBot.Utility.Config
{
    public class MainConfig
    {
        public String Token { get; set; }
        public List<String> MainPrefix { get; set; }
        public List<String> AdminPrefix { get; set; }

        public CommandConfig CommandConfig { get; set; }

        public DiscordConfig DiscordConfig { get; set; }
    }
}
