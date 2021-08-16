using DSharpPlus.CommandsNext;

using Newtonsoft.Json.Linq;

using System;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class PrefixRuleEvaluator
    {
        public Boolean EvaluatePrefixRule(CommandContext context, JToken objectData)
        {
            if(context?.Prefix == objectData.Value<String>())
                return true;
            return false;
        }
    }
}
