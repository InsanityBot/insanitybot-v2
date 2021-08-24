using System;

using Newtonsoft.Json.Linq;

namespace InsanityBot.Utility.Config
{
    public class TicketConfiguration : IConfiguration
    {
        public String DataVersion { get; set; }
        public JObject Configuration { get; set; }

        public Object this[String Identifier] => this.Configuration.SelectToken(Identifier).Value<Object>();

        public T Value<T>(String identifier) => this.Configuration.SelectToken(identifier).Value<T>();

        public void SetValue(String identifier, Object value) => this.Configuration[identifier] = JToken.FromObject(value);

        public TicketConfiguration()
        {
            this.DataVersion = "2.0.0";
            this.Configuration = new();
        }
    }
}
