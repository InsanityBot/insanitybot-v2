using System;

using Newtonsoft.Json.Linq;

namespace InsanityBot.Utility.Config
{
    public class LoggerConfiguration : IConfiguration
    {
        public JObject Configuration { get; set; }
        public String[] EventExclusions { get; set; }
        public Int32[] EventIdExclusions { get; set; }
        public String DataVersion { get; set; }

        public Object this[String Identifier] => this.Configuration.SelectToken(Identifier).Value<Object>();

        public T Value<T>(String identifier) => this.Configuration.SelectToken(identifier).Value<T>();

        public void SetValue(String identifier, Object value) => this.Configuration[identifier] = JToken.FromObject(value);
    }
}
