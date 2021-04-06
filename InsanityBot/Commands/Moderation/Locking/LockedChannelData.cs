using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.Commands.Moderation.Locking
{
    public struct LockedChannelData
    {
        public List<DiscordOverwrite> Overwrites { get; set; }
        public UInt64 ChannelId { get; set; }
    }
}
