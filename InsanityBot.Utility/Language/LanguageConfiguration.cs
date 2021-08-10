using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Language
{
    public class LanguageConfiguration : IConfiguration
    {
        public String DataVersion { get; set; }
        public JObject Configuration { get; set; }

        public LanguageConfiguration()
        {
            this.Configuration = new();
        }

        public String this[String Identifier]
        {
            get => Configuration.SelectToken(Identifier).Value<String>();
        }

        public void SetValue(String identifier, String value)
        {
            Configuration[identifier] = JToken.FromObject(value);
        }
    }
}
