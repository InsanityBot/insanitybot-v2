using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

namespace InsanityBot.Tickets.Validation
{
    public class ChannelDeleteValidator
    {
        public Task Validate(DiscordClient client, ChannelDeleteEventArgs args)
        {
            _ = Task.Run(() =>
            {
                if(!InsanityBot.TicketDaemon.Tickets.Any(xm => xm.Value.DiscordChannelId == args.Channel.Id))
                {
                    return;
                }

                InsanityBot.TicketDaemon.RemoveTicket(args.Channel.Id);
            });
            return Task.CompletedTask;
        }
    }
}
