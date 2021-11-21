namespace InsanityBot.MessageServices.Messages.Rules.Data;

using System;

using Newtonsoft.Json.Linq;

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