using System;

using Newtonsoft.Json.Linq;

namespace InsanityBot.Utility.Language
{
    public class LanguageConfiguration : IConfiguration
    {
        public String DataVersion { get; set; }
        public JObject Configuration { get; set; }

        public LanguageConfiguration() => this.Configuration = new();

        public String this[String Identifier] => this.Configuration.SelectToken(Identifier).Value<String>();

        public void SetValue(String identifier, String value) => this.Configuration[identifier] = JToken.FromObject(value);
    }
}
