using InsanityBot.MessageServices.Messages.Rules.Data;

using Newtonsoft.Json;

using System;
using System.IO;

namespace InsanityBot.MessageServices.Messages.Rules
{
    internal class MemberRuleTargetJsonConverter : JsonConverter<MemberRuleTarget>
    {
        public override MemberRuleTarget ReadJson(JsonReader reader, Type objectType, MemberRuleTarget existingValue, Boolean hasExistingValue, JsonSerializer serializer)
        {
            return ((String)reader.Value).ToLower() switch
            {
                "bot" => MemberRuleTarget.Bot,
                "owner" => MemberRuleTarget.Owner,
                "role" => MemberRuleTarget.Role,
                "id" => MemberRuleTarget.Id,
                _ => throw new InvalidDataException("Invalid JSON")
            };
        }
        public override void WriteJson(JsonWriter writer, MemberRuleTarget value, JsonSerializer serializer)
        {
            writer.WriteValue(value switch
            {
                MemberRuleTarget.Bot => "bot",
                MemberRuleTarget.Owner => "owner",
                MemberRuleTarget.Role => "role",
                MemberRuleTarget.Id => "id",
                _ => throw new InvalidProgramException("Non-existent enum field selected")
            });
        }
    }
}
