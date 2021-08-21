using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.Tickets.Commands;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Closure
{
    public class ClosingQueue
    {
        [JsonIgnore]
        private List<ClosingQueueEntry> __queue;

        [JsonIgnore]
        private List<UInt64> __channels;

        [JsonIgnore]
        private readonly Boolean __default_cancellable;

        [JsonIgnore]
        internal ClosingQueueHandler handler;

        public List<ClosingQueueEntry> Queue
        {
            get => this.__queue;
            set
            {
                this.__queue = value;
                this.__channels = (from i in this.__queue
                                   select i.ChannelId).ToList();
            }
        }

        public Task HandleCancellation(DiscordClient cl, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await this.HandleCancellationAsync(cl, e);
            });

            return Task.CompletedTask;
        }

        private async Task HandleCancellationAsync(DiscordClient cl, MessageCreateEventArgs e)
        {
            foreach(String v in InsanityBot.Config.Prefixes)
            {
                if(e.Message.Content.StartsWith($"{v}close"))
                {
                    return;
                }
            }

            if(e.Message.Author == cl.CurrentUser)
            {
                return;
            }

            if(this.__channels == null)
            {
                return;
            }

            if(!this.__channels.Contains(e.Channel.Id))
            {
                return;
            }

            ClosingQueueEntry entry = (from i in this.__queue
                                       where i.ChannelId == e.Channel.Id
                                       select i).First();

            if(!entry.Cancellable)
            {
                return;
            }

            this.__queue.Remove(entry);

            DiscordEmbedBuilder embedBuilder = InsanityBot.Embeds["insanitybot.tickets.close"]
                .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.cancelled_closing"].ReplaceValues(cl, e, entry.CloseDate));

            await e.Channel?.SendMessageAsync(embedBuilder.Build());
        }

        [JsonConstructor]
        public ClosingQueue()
        {
            this.__default_cancellable = TicketDaemon.Configuration.Value<Boolean>("insanitybot.tickets.closing_cancellable");

            if(this.__channels == null)
            {
                this.__channels = new();
            }

            if(this.__queue == null)
            {
                this.__queue = new();
            }
        }

        public void AddToQueue(UInt64 channelId, TimeSpan delay) => this.AddToQueue(channelId, delay, this.__default_cancellable);

        public void AddToQueue(UInt64 channelId, TimeSpan delay, Boolean cancellable)
        {
            this.__queue.Add(new()
            {
                Cancellable = cancellable,
                ChannelId = channelId,
                CloseDate = DateTimeOffset.Now + delay
            });

            this.__channels.Add(channelId);
        }
    }
}
