using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using DSharpPlus.Entities;

using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

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
            using StreamReader reader = new StreamReader(File.OpenRead(Filename));

            MainConfiguration config = (MainConfiguration)JsonConvert.DeserializeObject(reader.ReadToEnd());
            return (MainConfiguration)DataFixerLower.UpgradeData(config);
        }

        public void Serialize(MainConfiguration Config, String Filename)
        {
            using StreamWriter writer = new StreamWriter(File.OpenWrite(Filename));
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
