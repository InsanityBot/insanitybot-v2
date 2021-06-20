using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace InsanityBot.Tickets
{
	public record TicketPreset
	{
		[JsonProperty(Required = Required.Always)]
		public UInt64 Category { get; set; }

		[DefaultValue("ticket-{username:4}-{discriminator}")]
		[JsonProperty(Required = Required.Always)]
		public String NameFormat { get; set; }

		[JsonProperty(Required = Required.AllowNull)]
		public String Topic { get; set; }

		[JsonProperty(Required = Required.AllowNull)]
		public TicketAccess AccessRules { get; set; }

		[JsonProperty(Required = Required.Always)]
		public TicketSettings Settings { get; set; }

		public String[] CreationMessages { get; set; }
		public String[] CreationEmbeds { get; set; }
	}
}
