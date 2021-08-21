using DSharpPlus.Entities;

using System;
using System.Threading.Tasks;

namespace InsanityBot.MessageServices.Messages
{
    public interface ILoggingGateway
    {
        public Task SendMessage(DiscordEmbed embed);
        public Task SendMessage(String content);
        public Task SendMessage(DiscordMessageBuilder builder);
    }
}
