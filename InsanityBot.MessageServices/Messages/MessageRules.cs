using DSharpPlus.Entities;

using InsanityBot.MessageServices.Messages.Rules;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.MessageServices.Messages
{
    public class MessageRules
    {
        public Dictionary<LogEvent, DiscordChannel> Defaults { get; set; }
        public Dictionary<LogEvent, LoggerRule[]> Rules { get; set; }
        public Dictionary<UInt64, ILoggingGateway> Channels { get; set; }

        private readonly DiscordGuild _guild;

        public MessageRules(DiscordGuild guild)
        {
            _guild = guild;
        }
    }
}
