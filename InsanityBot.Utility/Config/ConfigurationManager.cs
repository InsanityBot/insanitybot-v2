﻿namespace InsanityBot.Utility.Config;
using System;
using System.IO;

using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json;

public class ConfigurationManager
{
	public T Deserialize<T>(String Filename)
		where T : IConfiguration
	{
		using StreamReader reader = new(File.OpenRead(Filename));

		T config = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
		config = (T)DataFixerLower.UpgradeData(config);
		reader.Close();

		this.Serialize(config, Filename);
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