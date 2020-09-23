using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility.Config
{
    public class MainConfiguration : IConfiguration
    {
        public String DataVersion { get; set; }
        public Dictionary<String, Object> Configuration { get; set; }

        public String Token { get; set; }
        public String GuildId { get; set; }
    }
}
