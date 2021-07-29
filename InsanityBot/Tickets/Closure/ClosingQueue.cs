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
        
        public async Task HandleCancellation(DiscordClient cl, MessageCreateEventArgs e)
        {
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
    }
}
