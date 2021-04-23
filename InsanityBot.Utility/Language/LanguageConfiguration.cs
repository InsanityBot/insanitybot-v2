using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Language
{
    public class LanguageConfiguration : IConfiguration<String>
    {
        public String DataVersion { get; set; }
        public Dictionary<String, String> Configuration { get; set; }

        public LanguageConfiguration()
        {
            DataVersion = "2.0.0-dev.00017";
            Configuration = new Dictionary<String, String>();
        }

        public String this[String index]
        {
            get => Configuration[index];
            set => Configuration[index] = value;
        }
    }
}
