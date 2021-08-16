using DSharpPlus.CommandsNext;

using Newtonsoft.Json.Linq;

using System;
using System.Linq;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class MemberRuleEvaluator
    {
        public Boolean EvaluateBotRule(CommandContext ctx, JToken objectData)
        {
            return ctx.Member?.IsBot == objectData.Value<Boolean>();
        }

        public Boolean EvaluateOwnerRule(CommandContext ctx, JToken objectData)
        {
            return ctx.Member?.IsOwner == objectData.Value<Boolean>();
        }

        public Boolean EvaluateIdRule(CommandContext ctx, JToken objectData)
        {
            return ctx.Member?.Id == objectData.Value<UInt64>();
        }

        public Boolean EvaluateRoleIdRule(CommandContext ctx, JToken objectData)
        {
            return ctx.Member?.Roles.Any(xm => xm.Id == objectData.Value<UInt64>()) ?? false;
        }
    }
}
