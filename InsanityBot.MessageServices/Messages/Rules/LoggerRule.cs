﻿namespace InsanityBot.MessageServices.Messages.Rules;
using System;

using InsanityBot.MessageServices.Messages.Rules.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoggerRule
{
	[JsonProperty("target")]
	[JsonRequired]
	[JsonConverter(typeof(RuleTargetJsonConverter))]
	public RuleTarget Target { get; set; }

	[JsonProperty("memberTarget")]
	[JsonConverter(typeof(MemberRuleTargetJsonConverter))]
	public MemberRuleTarget? MemberTarget { get; set; } = null;

	[JsonProperty("channelTarget")]
	[JsonConverter(typeof(ChannelRuleTargetJsonConverter))]
	public ChannelRuleTarget? ChannelTarget { get; set; } = null;

	[JsonProperty("parameter")]
	[JsonRequired]
	public JObject RuleParameter { get; set; }

	[JsonProperty("allowAction")]
	[JsonRequired]
	public Boolean Allow { get; set; }

	[JsonProperty("channel")]
	[JsonRequired]
	public UInt64 Channel { get; set; }
}