using System;
using System.Linq;

using DSharpPlus.CommandsNext;

using Newtonsoft.Json.Linq;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class CommandRuleEvaluator
    {
        public Boolean EvaluateCommandRule(Command cmd, JToken objectData)
        {
            if(cmd?.Name == objectData.Value<String>())
            {
                return true;
            }

            if(cmd?.Aliases.Contains(objectData.Value<String>()) ?? false)
            {
                return true;
            }

            return false;
        }
    }
}
