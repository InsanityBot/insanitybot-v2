using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Newtonsoft.Json.Linq;

using System;
using System.Linq;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class MemberRuleEvaluator
    {
        public Boolean EvaluateBotRule(DiscordMember member, JToken objectData)
        {
            return member?.IsBot == objectData.Value<Boolean>();
        }

        public Boolean EvaluateOwnerRule(DiscordMember member, JToken objectData)
        {
            return member?.IsOwner == objectData.Value<Boolean>();
        }

        public Boolean EvaluateIdRule(DiscordMember member, JToken objectData)
        {
            return member?.Id == objectData.Value<UInt64>();
        }

        public Boolean EvaluateRoleIdRule(DiscordMember member, JToken objectData)
        {
            return member?.Roles.Any(xm => xm.Id == objectData.Value<UInt64>()) ?? false;
        }
    }
}
