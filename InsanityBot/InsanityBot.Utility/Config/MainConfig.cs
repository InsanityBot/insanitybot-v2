using System;
using System.Collections.Generic;
using System.Text;

using InsanityBot.Utility.Config.Reference;

namespace InsanityBot.Utility.Config
{
    public class MainConfig
    {
        public String Token { get; set; }

        public DiscordConfig DiscordConfig { get; set; }
        public CommandConfig CommandConfig { get; set; }
    }
}
