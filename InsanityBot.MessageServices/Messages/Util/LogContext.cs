using System;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace InsanityBot.MessageServices.Messages.Util
{
    public class LogContext
    {
        public DiscordMember Member { get; set; }
        public DiscordChannel Channel { get; set; }
        public DiscordMessage Message { get; set; }
        public Command Command { get; set; }
        public String Prefix { get; set; }

        public LogContext(DiscordUser actor, DiscordChannel channel, Command cmd, DiscordMessage message, String prefix = null)
        {
            this.Member = (DiscordMember)actor;
            this.Channel = channel;
            this.Command = cmd;
            this.Message = message;
            this.Prefix = prefix;
        }
    }
}
