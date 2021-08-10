using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

using System;
using System.IO;

namespace InsanityBot.Utility.Config
{
    public class ConfigurationManager
    {
        public T Deserialize<T>(String Filename)
            where T: IConfiguration
        {
            using StreamReader reader = new(File.OpenRead(Filename));

            T config = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            config = (T)DataFixerLower.UpgradeData(config);
            reader.Close();

            this.Serialize(config, "./config/main.json");
            return config;
        }

        public void Serialize<T>(T Config, String Filename)
        {
            using StreamWriter writer = new(File.OpenWrite(Filename));
            writer.BaseStream.SetLength(0);
            writer.Flush();
            writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }
}
