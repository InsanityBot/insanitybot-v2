
using System;

using Newtonsoft.Json.Linq;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class PrefixRuleEvaluator
    {
        public Boolean EvaluatePrefixRule(String prefix, JToken objectData)
        {
            if(prefix == objectData.Value<String>())
            {
                return true;
            }

            return false;
        }
    }
}
