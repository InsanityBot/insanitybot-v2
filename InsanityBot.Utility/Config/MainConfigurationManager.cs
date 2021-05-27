using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

using System;
using System.IO;

namespace InsanityBot.Utility.Config
{
    public class MainConfigurationManager : IConfigSerializer<MainConfiguration, Object>,
        IConfigBuilder<MainConfiguration, MainConfigurationManager, Object>
    {
        public MainConfiguration Config { get; set; }

        public MainConfigurationManager AddConfigEntry(String Identifier, Object DefaultValue)
        {
            Config.Configuration.Add(Identifier, DefaultValue);
            return this;
        }

        public MainConfigurationManager RemoveConfigEntry(String Identifier)
        {
            Config.Configuration.Remove(Identifier);
            return this;
        }

        public MainConfiguration Deserialize(String Filename)
        {
            using StreamReader reader = new(File.OpenRead(Filename));

            MainConfiguration config = JsonConvert.DeserializeObject<MainConfiguration>(reader.ReadToEnd());
            config = (MainConfiguration)DataFixerLower.UpgradeData(config);
            reader.Close();

            Serialize(config, "./config/main.json");
            return config;
        }

        public void Serialize(MainConfiguration Config, String Filename)
        {
            using StreamWriter writer = new(File.OpenWrite(Filename));
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
