using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.MessageServices.Messages
{
    public class LoggingWebhook : ILoggingGateway
    {
        private readonly DiscordWebhook webhook;

        public LoggingWebhook(DiscordChannel channel)
        {
            if(channel == null)
            {
                webhook = null;
                return;
            }

            IReadOnlyList<DiscordWebhook> webhooks = channel?.GetWebhooksAsync().GetAwaiter().GetResult();

            if(!webhooks.Any(xm => xm.Name == "InsanityBot"))
            {
                this.webhook = channel.CreateWebhookAsync("InsanityBot").GetAwaiter().GetResult();
                return;
            }

            this.webhook = webhooks.First(xm => xm.Name == "InsanityBot");
        }

        public async Task SendMessage(DiscordEmbed embed)
        {
            if(this.webhook == null)
            {
                return;
            }

            await this.webhook?.ExecuteAsync(new DiscordWebhookBuilder()
                .AddEmbed(embed));
        }
        public async Task SendMessage(String content)
        {
            if(this.webhook == null)
            {
                return;
            }

            await this.webhook?.ExecuteAsync(new DiscordWebhookBuilder()
                .WithContent(content));
        }
        public async Task SendMessage(DiscordMessageBuilder builder)
        {
            if(this.webhook == null)
            {
                return;
            }

            await this.webhook?.ExecuteAsync(new DiscordWebhookBuilder()
                .WithContent(builder.Content)
                .AddEmbeds(builder.Embeds));
        }

        public static ILoggingGateway Empty => new LoggingWebhook(null);
    }
}
