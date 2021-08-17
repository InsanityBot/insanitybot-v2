using InsanityBot.MessageServices.Messages.Rules.Data;

using Newtonsoft.Json;

using System;
using System.IO;

namespace InsanityBot.MessageServices.Messages.Rules
{
    internal class ChannelRuleTargetJsonConverter : JsonConverter<ChannelRuleTarget>
    {
        public override ChannelRuleTarget ReadJson(JsonReader reader, Type objectType, ChannelRuleTarget existingValue, Boolean hasExistingValue, JsonSerializer serializer)
        {
            return ((String)reader.Value).ToLower() switch
            {
                "id" => ChannelRuleTarget.Id,
                "fullname" or "name" => ChannelRuleTarget.FullName,
                "namecontains" or "contains" => ChannelRuleTarget.NameContains,
                "namestartswith" or "startswith" => ChannelRuleTarget.NameStartsWith,
                "category" or "parent" => ChannelRuleTarget.Category,
                _ => throw new InvalidDataException("Invalid JSON")
            };
        }
        public override void WriteJson(JsonWriter writer, ChannelRuleTarget value, JsonSerializer serializer)
        {
            writer.WriteValue(value switch
            {
                ChannelRuleTarget.Id => "id",
                ChannelRuleTarget.FullName => "fullname",
                ChannelRuleTarget.NameContains => "namecontains",
                ChannelRuleTarget.NameStartsWith => "namestartswith",
                ChannelRuleTarget.Category => "category",
                _ => throw new InvalidProgramException("Non-existent enum field selected")
            });
        }
    }
}
