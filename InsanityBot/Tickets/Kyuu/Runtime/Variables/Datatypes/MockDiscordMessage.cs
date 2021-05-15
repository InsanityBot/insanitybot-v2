using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Runtime.Variables.Datatypes
{
    public struct MockDiscordMessage
    {
        public String MessageContent { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, 
            NullValueHandling = NullValueHandling.Include, Required = Required.Default)]
        public UInt64? Reply { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            NullValueHandling = NullValueHandling.Include, Required = Required.Default)]
        public Boolean? MentionReply { get; set; }
    }
}
