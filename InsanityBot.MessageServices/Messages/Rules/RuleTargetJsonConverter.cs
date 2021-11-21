namespace InsanityBot.MessageServices.Messages.Rules;
using System;
using System.IO;

using InsanityBot.MessageServices.Messages.Rules.Data;

using Newtonsoft.Json;

internal class RuleTargetJsonConverter : JsonConverter<RuleTarget>
{
	public override RuleTarget ReadJson(JsonReader reader, Type objectType, RuleTarget existingValue, Boolean hasExistingValue, JsonSerializer serializer)
	{
		return ((String)reader.Value).ToLower() switch
		{
			"channel" => RuleTarget.Channel,
			"member" => RuleTarget.Member,
			"command" => RuleTarget.Command,
			"prefix" => RuleTarget.Prefix,
			_ => throw new InvalidDataException("Invalid JSON")
		};
	}
	public override void WriteJson(JsonWriter writer, RuleTarget value, JsonSerializer serializer)
	{
		writer.WriteValue(value switch
		{
			RuleTarget.Channel => "channel",
			RuleTarget.Member => "member",
			RuleTarget.Command => "command",
			RuleTarget.Prefix => "prefix",
			_ => throw new InvalidProgramException("Non-existent enum field selected")
		});
	}
}