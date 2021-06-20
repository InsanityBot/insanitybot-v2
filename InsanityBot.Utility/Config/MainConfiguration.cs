using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Config
{
	public class MainConfiguration : IConfiguration<Object>
	{
		public String DataVersion { get; set; }
		public Dictionary<String, Object> Configuration { get; set; }

		public List<String> Prefixes { get; set; }

		public String Token { get; set; }
		public UInt64 GuildId { get; set; }

		public Object this[String Identifier]
		{
			get => this.Configuration[Identifier];
			set => this.Configuration[Identifier] = value;
		}

		public MainConfiguration()
		{
			this.DataVersion = "2.0.0-dev.00017";
			this.Configuration = new Dictionary<String, Object>();
			this.Token = " ";
			this.GuildId = 0;
		}
	}
}
