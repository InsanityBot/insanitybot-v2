using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.MessageServices.Messages
{
    public class LoggingChannel : ILoggingGateway
    {
        private readonly DiscordChannel underlying;

        public LoggingChannel(DiscordChannel channel) => this.underlying = channel;

        public async Task SendMessage(DiscordEmbed embed) => await this.underlying?.SendMessageAsync(embed);
        public async Task SendMessage(String content) => await this.underlying?.SendMessageAsync(content);
        public async Task SendMessage(DiscordMessageBuilder builder) => await this.underlying?.SendMessageAsync(builder);

        public static ILoggingGateway Empty => new LoggingChannel(null);
    }
}
