using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using DSharpPlus.Entities;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Config
{
    public class MainConfigurationManager : IConfigSerializer<MainConfiguration>, IConfigBuilder<MainConfiguration, MainConfigurationManager>
    {
        public MainConfiguration Config { get; set; }

        public MainConfigurationManager AddConfigEntry<T>(String Identifier, T DefaultValue)
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
            return JsonConvert.DeserializeObject<MainConfiguration>(reader.ReadToEnd());
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
