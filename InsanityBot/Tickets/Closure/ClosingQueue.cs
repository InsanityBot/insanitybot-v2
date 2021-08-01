using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.Tickets.Commands;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            get => __queue;
            set
            {
                __queue = value;
                __channels = (from i in __queue
                              select i.ChannelId).ToList();
            }
        }
        
        public Task HandleCancellation(DiscordClient cl, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await HandleCancellationAsync(cl, e);
            });

            return Task.CompletedTask;
        }

        private async Task HandleCancellationAsync(DiscordClient cl, MessageCreateEventArgs e)
        {
            foreach(var v in InsanityBot.Config.Prefixes)
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

            if(__channels == null)
            {
                return;
            }

            if(!__channels.Contains(e.Channel.Id))
            {
                return;
            }

            ClosingQueueEntry entry = (from i in __queue
                                       where i.ChannelId == e.Channel.Id
                                       select i).First();

            if(!entry.Cancellable)
            {
                return;
            }

            __queue.Remove(entry);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Description = InsanityBot.LanguageConfig["insanitybot.tickets.cancelled_closing"].ReplaceValues(cl, e, entry.CloseDate),
                Color = DiscordColor.SpringGreen
            };

            await e.Channel.SendMessageAsync(embedBuilder.Build());
        }

        [JsonConstructor]
        public ClosingQueue()
        {
            __default_cancellable = (Boolean)TicketDaemon.Configuration["insanitybot.tickets.closing_cancellable"];

            if(__channels == null)
            {
                __channels = new();
            }

            if(__queue == null)
            {
                __queue = new();
            }
        }

        public void AddToQueue(UInt64 channelId, TimeSpan delay) => AddToQueue(channelId, delay, __default_cancellable);

        public void AddToQueue(UInt64 channelId, TimeSpan delay, Boolean cancellable)
        {
            __queue.Add(new()
            {
                Cancellable = cancellable,
                ChannelId = channelId,
                CloseDate = DateTime.Now + delay
            });

            __channels.Add(channelId);
        }
    }
}
