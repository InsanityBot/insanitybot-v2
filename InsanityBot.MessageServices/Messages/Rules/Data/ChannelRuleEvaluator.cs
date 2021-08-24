using System;

using DSharpPlus.Entities;

using Newtonsoft.Json.Linq;

namespace InsanityBot.MessageServices.Messages.Rules.Data
{
    public class ChannelRuleEvaluator
    {
        public Boolean EvaluateIdRule(DiscordChannel channel, JToken objectData)
        {
            if(objectData.Value<UInt64>() == channel?.Id)
            {
                return true;
            }

            return false;
        }

        public Boolean EvaluateFullNameRule(DiscordChannel channel, JToken objectData)
        {
            if(objectData.Value<String>() == channel?.Name)
            {
                return true;
            }

            return false;
        }

        public Boolean EvaluateNameContainsRule(DiscordChannel channel, JToken objectData)
        {
            if(channel?.Name.Contains(objectData.Value<String>()) ?? false)
            {
                return true;
            }

            return false;
        }

        public Boolean EvaluateNameStartsWithRule(DiscordChannel channel, JToken objectData)
        {
            if(channel?.Name.StartsWith(objectData.Value<String>()) ?? false)
            {
                return true;
            }

            return false;
        }

        public Boolean EvaluateCategoryRule(DiscordChannel channel, JToken objectData)
        {
            if(channel?.Parent.Id == objectData.Value<UInt64>())
            {
                return true;
            }

            return false;
        }
    }
}
