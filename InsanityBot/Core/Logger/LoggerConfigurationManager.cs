using InsanityBot.Utility;
using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

using System;
using System.IO;

namespace InsanityBot.Core.Logger
{
    public class LoggerConfigurationManager : IConfigBuilder<LoggerConfiguration, LoggerConfigurationManager, Object>,
        IConfigSerializer<LoggerConfiguration, Object>
    {
        public LoggerConfiguration Config { get; set; }

        public LoggerConfigurationManager AddConfigEntry(String Identifier, Object DefaultValue)
        {
            Config.Configuration.Add(Identifier, DefaultValue);
            return this;
        }

        public LoggerConfiguration Deserialize(String Filename)
        {
            StreamReader reader = new(File.OpenRead(Filename));

            LoggerConfiguration config = JsonConvert.DeserializeObject<LoggerConfiguration>(reader.ReadToEnd());
            config = (LoggerConfiguration)DataFixerLower.UpgradeData(config);
            reader.Close();

            Serialize(config, "./config/logger.json");
            return config;
        }

        public LoggerConfigurationManager RemoveConfigEntry(String Identifier)
        {
            Config.Configuration.Remove(Identifier);
            return this;
        }

        public void Serialize(LoggerConfiguration Config, String Filename)
        {
            using StreamWriter writer = new(File.OpenWrite(Filename));
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
