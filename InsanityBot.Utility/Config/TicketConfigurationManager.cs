using Newtonsoft.Json;

using System;
using System.IO;

namespace InsanityBot.Utility.Config
{
    public class TicketConfigurationManager : IConfigSerializer<TicketConfiguration, Object>,
        IConfigBuilder<TicketConfiguration, TicketConfigurationManager, Object>
    {
        public TicketConfiguration Config { get; set; }

        public TicketConfigurationManager AddConfigEntry(String Identifier, Object DefaultValue)
        {
            Config.Configuration.Add(Identifier, DefaultValue);
            return this;
        }

        public TicketConfigurationManager RemoveConfigEntry(String Identifier)
        {
            Config.Configuration.Remove(Identifier);
            return this;
        }

        public TicketConfiguration Deserialize(String Filename)
        {
            using StreamReader reader = new(File.OpenRead(Filename));
            return JsonConvert.DeserializeObject<TicketConfiguration>(reader.ReadToEnd());
        }

        public void Serialize(TicketConfiguration Config, String Filename)
        {
            using StreamWriter writer = new(File.OpenWrite(Filename));
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
