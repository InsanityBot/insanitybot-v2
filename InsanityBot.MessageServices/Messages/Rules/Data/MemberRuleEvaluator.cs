using System;
using System.Linq;

using DSharpPlus.Entities;

using Newtonsoft.Json.Linq;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class MemberRuleEvaluator
    {
        public Boolean EvaluateBotRule(DiscordMember member, JToken objectData) => member?.IsBot == objectData.Value<Boolean>();

        public Boolean EvaluateOwnerRule(DiscordMember member, JToken objectData) => member?.IsOwner == objectData.Value<Boolean>();

        public Boolean EvaluateIdRule(DiscordMember member, JToken objectData) => member?.Id == objectData.Value<UInt64>();

        public Boolean EvaluateRoleIdRule(DiscordMember member, JToken objectData) => member?.Roles.Any(xm => xm.Id == objectData.Value<UInt64>()) ?? false;
    }
}
