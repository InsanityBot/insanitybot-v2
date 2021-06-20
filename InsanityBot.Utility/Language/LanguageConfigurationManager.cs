using System;
using System.IO;

using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Language
{
	public class LanguageConfigurationManager : IConfigBuilder<LanguageConfiguration, LanguageConfigurationManager, String>,
		IConfigSerializer<LanguageConfiguration, String>
	{
		public LanguageConfiguration Config { get; set; }

		public LanguageConfigurationManager AddConfigEntry(String Identifier, String DefaultValue)
		{
			this.Config.Configuration.Add(Identifier, DefaultValue);
			return this;
		}

		public LanguageConfiguration Deserialize(String Filename)
		{
			StreamReader reader = new(File.OpenRead(Filename));

			LanguageConfiguration config = JsonConvert.DeserializeObject<LanguageConfiguration>(reader.ReadToEnd());
			config = (LanguageConfiguration)DataFixerLower.UpgradeData(config);
			reader.Close();

			this.Serialize(config, "./config/lang.json");
			return config;
		}

		public LanguageConfigurationManager RemoveConfigEntry(String Identifier)
		{
			this.Config.Configuration.Remove(Identifier);
			return this;
		}

		public void Serialize(LanguageConfiguration Config, String Filename)
		{
			using StreamWriter writer = new(File.OpenWrite(Filename));
			writer.BaseStream.SetLength(0);
			writer.Flush();
			writer.Write(JsonConvert.SerializeObject(Config, Formatting.Indented));
		}
	}
}
