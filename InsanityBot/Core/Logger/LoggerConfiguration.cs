using System;
using System.Collections.Generic;

using InsanityBot.Utility;

namespace InsanityBot.Core.Logger
{
	public class LoggerConfiguration : IConfiguration<Object>
	{
		public Dictionary<String, Object> Configuration { get; set; }
		public String[] EventExclusions { get; set; }
		public Int32[] EventIdExclusions { get; set; }
		public String DataVersion { get; set; }

		public Object this[String key]
		{
			get => this.Configuration[key];
			set => this.Configuration[key] = value;
		}
	}
}
