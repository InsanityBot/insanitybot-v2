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
            Channels = new();
            foreach((ModlogMessageType type, DiscordChannel channel) in entries)
            {
                Channels.Add(type, channel);
            }
        }

        public Task QueueMessage(ModlogMessageType type, DiscordMessageBuilder message)
        {
            _ = Task.Run(() =>
            {
                Channels[type].SendMessageAsync(message);
            });
            return Task.CompletedTask;
        }
    }
}
