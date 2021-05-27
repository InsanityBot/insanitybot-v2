using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility;
using InsanityBot.Utility.Config;
using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

namespace InsanityBot.Tickets.Daemon.Config
{
    class TicketConfigurationManager : IConfigSerializer<TicketConfiguration, Object>,
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

            TicketConfiguration config = JsonConvert.DeserializeObject<TicketConfiguration>(reader.ReadToEnd());
            config = (TicketConfiguration)DataFixerLower.UpgradeData(config);
            reader.Close();

            Serialize(config, "./config/ticket.json");
            return config;
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
