using System.IO;
using System.Xml.Serialization;

namespace InsanityBot.Utility.Config
{
    /// <summary>
    /// Main class for serializing and deserializing config files. Also holds a static copy of the default config values.
    /// </summary>
    public static class MainConfigManager
    {
        /// <summary>
        /// Main default configuration. Used to create a new configuration from defaults.
        /// </summary>
        public static MainConfig DefaultMainConfiguration { get; } = new MainConfig
        {

        };

        /// <summary>
        /// Reads the main configuration file and deserializes it into a MainConfig instance
        /// </summary>
        public static MainConfig DeserializeMainConfiguration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainConfig));
            StreamReader reader = new StreamReader("./config/main.xml");

            MainConfig value = (MainConfig)serializer.Deserialize(reader);
            reader.Close();
            return value;
        }

        /// <summary>
        /// Serializes the selected config to the main configuration file, overwriting existing data in the process
        /// </summary>
        /// <param name="config">Configuration instance to be serialized</param>
        public static void SerializeMainConfiguration(MainConfig config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainConfig));
            FileStream writer = new FileStream("./config/main.xml", FileMode.Truncate);

            serializer.Serialize(writer, config);
            writer.Close();
        }

        /// <summary>
        /// Serializes the default main config to the main configuration file, overwriting existing data in the process
        /// </summary>
        public static void SerializeMainConfiguration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainConfig));
            FileStream writer = new FileStream("./config/main.xml", FileMode.Truncate);

            serializer.Serialize(writer, DefaultMainConfiguration);
            writer.Close();
        }
    }
}
