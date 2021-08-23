using DSharpPlus.Entities;

using System;
using System.Threading.Tasks;

namespace InsanityBot.MessageServices.Messages
{
    public class LoggingChannel : ILoggingGateway
    {
        private readonly DiscordChannel underlying;

        public LoggingChannel(DiscordChannel channel)
        {
            underlying = channel;
        }

        public async Task SendMessage(DiscordEmbed embed)
        {
            await underlying?.SendMessageAsync(embed);
        }
        public async Task SendMessage(String content)
        {
            await underlying?.SendMessageAsync(content);
        }
        public async Task SendMessage(DiscordMessageBuilder builder)
        {
            await underlying?.SendMessageAsync(builder);
        }

        public static ILoggingGateway Empty
        {
            get => new LoggingChannel(null);
        }
    }
}
