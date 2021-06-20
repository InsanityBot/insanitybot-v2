using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Config
{
	public class ApplicationConfiguration : IConfiguration<Object>
	{
		public String DataVersion { get; set; }
		public Dictionary<String, Object> Configuration { get; set; }
		public String Identifier { get; set; }
	}
}
