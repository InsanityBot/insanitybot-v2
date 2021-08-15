using DSharpPlus.CommandsNext;

using Newtonsoft.Json.Linq;

using System;
using System.Linq;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class CommandRuleEvaluator
    {
        public Boolean EvaluateCommandRule(CommandContext ctx, JToken objectData)
        {
            if(ctx.Command.Name == objectData.Value<String>())
                return true;
            if(ctx.Command.Aliases.Contains(objectData.Value<String>()))
                return true;
            return false;
        }
    }
}
