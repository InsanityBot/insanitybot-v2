using DSharpPlus.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsanityBot.Core.Services.Internal.Modlogs
{
    public class ModlogMessageQueue
    {
        private Dictionary<ModlogMessageType, DiscordChannel> Channels { get; set; }

        public ModlogMessageQueue(params (ModlogMessageType type, DiscordChannel channel)[] entries)
        {
            this.Channels = new();
            foreach((ModlogMessageType type, DiscordChannel channel) in entries)
            {
                this.Channels.Add(type, channel);
            }
        }

        public Task QueueMessage(ModlogMessageType type, DiscordMessageBuilder message)
        {
            _ = Task.Run(() =>
            {
                this.Channels[type].SendMessageAsync(message);
            });
            return Task.CompletedTask;
        }
    }
}
