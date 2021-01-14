using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility.Config
{
    public class MainConfiguration : IConfiguration<Object>
    {
        public String DataVersion { get; set; }
        public Dictionary<String, Object> Configuration { get; set; }

        public List<String> Prefixes { get; set; }

        public String Token { get; set; } 
        public UInt64 GuildId { get; set; } 

        public Object this[String Identifier]
        {
            get => Configuration[Identifier];
            set => Configuration[Identifier] = value;
        }

        public MainConfiguration()
        {
            DataVersion = "2.0.0-dev.00013";
            Configuration = new Dictionary<String, Object>();
            Token = " ";
            GuildId = 0;
        }
    }
}
