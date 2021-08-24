using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.MessageServices.Messages.Util
{
    public class LogContext
    {
        public DiscordMember Member { get; set; }
        public DiscordChannel Channel { get; set; }
        public DiscordMessage Message { get; set; }
        public Command Command { get; set; }
        public String Prefix { get;set; }

        public LogContext(DiscordUser actor, DiscordChannel channel, Command cmd, DiscordMessage message, String prefix = null)
        {
            Member = (DiscordMember)actor;
            Channel = channel;
            Command = cmd;
            Message = message;
            Prefix = prefix;
        }
    }
}
