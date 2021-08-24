using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace InsanityBot.Utility.Config
{
    public class MainConfiguration : IConfiguration
    {
        public String DataVersion { get; set; }
        public JObject Configuration { get; set; }

        public List<String> Prefixes { get; set; }

        public String Token { get; set; }
        public UInt64 GuildId { get; set; }

        public Object this[String Identifier] => this.Configuration.SelectToken(Identifier)?.Value<Object>();

        public T Value<T>(String identifier) => this.Configuration.SelectToken(identifier).Value<T>();

        public void SetValue(String identifier, Object value) => this.Configuration[identifier] = JToken.FromObject(value);

        public MainConfiguration()
        {
            this.Configuration = new();
            this.Token = " ";
            this.GuildId = 0;
        }
    }
}
