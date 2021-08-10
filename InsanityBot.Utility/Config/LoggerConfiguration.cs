using Newtonsoft.Json.Linq;

using System;

namespace InsanityBot.Utility.Config
{
    public class LoggerConfiguration : IConfiguration
    {
        public JObject Configuration { get; set; }
        public String[] EventExclusions { get; set; }
        public Int32[] EventIdExclusions { get; set; }
        public String DataVersion { get; set; }

        public Object this[String Identifier]
        {
            get => Configuration.SelectToken(Identifier).Value<Object>();
        }

        public T Value<T>(String identifier)
        {
            return Configuration.SelectToken(identifier).Value<T>();
        }

        public void SetValue(String identifier, Object value)
        {
            Configuration[identifier] = JToken.FromObject(value);
        }
    }
}
