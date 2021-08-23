using DSharpPlus;
using DSharpPlus.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.MessageServices.Messages
{
    public class LoggingWebhook : ILoggingGateway
    {
        private readonly DiscordWebhook webhook;

        public LoggingWebhook(DiscordChannel channel)
        {
            var webhooks = channel?.GetWebhooksAsync().GetAwaiter().GetResult();

            if(!webhooks.Any(xm => xm.Name == "InsanityBot"))
            {
                webhook = channel.CreateWebhookAsync("InsanityBot").GetAwaiter().GetResult();
                return;
            }

            webhook = webhooks.First(xm => xm.Name == "InsanityBot");
        }

        public async Task SendMessage(DiscordEmbed embed)
        {
            await webhook?.ExecuteAsync(new DiscordWebhookBuilder()
                .AddEmbed(embed));
        }
        public async Task SendMessage(String content)
        {
            await webhook?.ExecuteAsync(new DiscordWebhookBuilder()
                .WithContent(content));
        }
        public async Task SendMessage(DiscordMessageBuilder builder)
        {
            await webhook?.ExecuteAsync(new DiscordWebhookBuilder()
                .WithContent(builder.Content)
                .AddEmbeds(builder.Embeds));
        }

        public static ILoggingGateway Empty
        {
            get => new LoggingWebhook(null);
        }
    }
}
