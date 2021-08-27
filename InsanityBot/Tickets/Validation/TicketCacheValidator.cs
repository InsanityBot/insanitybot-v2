using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

namespace InsanityBot.Tickets.Validation
{
    public class TicketCacheValidator
    {
        public Task Validate(DiscordClient sender, GuildDownloadCompletedEventArgs args)
        {
            if(!args.Guilds.ContainsKey(InsanityBot.HomeGuild.Id))
                return Task.CompletedTask;

            List<UInt64> cacheIds = InsanityBot.TicketDaemon.Tickets.Select(xm => xm.Value.DiscordChannelId).ToList();
            List<UInt64> channelIds = args.Guilds[InsanityBot.HomeGuild.Id].Channels.Select(xm => xm.Key).ToList();

            foreach(var v in cacheIds)
            {
                if(!channelIds.Contains(v))
                {
                    InsanityBot.TicketDaemon.RemoveTicket(v);
                }
            }

            InsanityBot.TicketDaemon.SaveAll();

            return Task.CompletedTask;
        }
    }
}
