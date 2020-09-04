using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

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
            FileStream writer = new FileStream("./config/tickets.xml", FileMode.Truncate);
            XmlSerializer serializer = new XmlSerializer(typeof(TicketConfig));

            serializer.Serialize(writer, config);
            writer.Close();
        }

        /// <summary>
        /// Returns the deserialized ticket config
        /// </summary>
        public static TicketConfig Deserialize()
        {
            StreamReader reader = new StreamReader("./config/tickets.xml");
            XmlSerializer deserializer = new XmlSerializer(typeof(TicketConfig));

            var returnValue = (TicketConfig)deserializer.Deserialize(reader);
            reader.Close();
            return returnValue;
        }
    }
}
