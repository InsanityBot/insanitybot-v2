using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Config
{
    /// <summary>
    /// Management class to serialize and deserialize ticket config
    /// </summary>
    public static class TicketConfigManager
    {
        /// <summary>
        /// Serializes a given config to the file, overwriting all data in the process
        /// </summary>
        public static void Serialize(TicketConfig config)
        {
            FileStream file = new FileStream("./config/tickets.xml", FileMode.Truncate);
            StreamWriter writer = new StreamWriter(file);

            writer.Write(JsonConvert.SerializeObject(config));

            file.Close();
        }

        /// <summary>
        /// Returns the deserialized ticket config
        /// </summary>
        public static TicketConfig Deserialize()
        {
            StreamReader reader = new StreamReader("./config/tickets.xml");

            return (TicketConfig)JsonConvert.DeserializeObject(reader.ReadToEnd());
        }
    }
}
