using DSharpPlus.Entities;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace InsanityBot.Tickets.Closure
{
    public class ClosingQueueHandler
    {
        private Timer Countdown;

        public void Start()
        {
            this.Countdown = new()
            {
                Interval = 2000,
                AutoReset = false
            };
            this.Countdown.Elapsed += this.CountdownElapsed;

            this.Countdown.Start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CountdownElapsed(Object sender, ElapsedEventArgs e) => _ = this.CountdownElapsedAsync();

        private async Task CountdownElapsedAsync()
        {
            DateTime compareAgainst = DateTime.Now;

            for(Int32 i = 0; i < InsanityBot.TicketDaemon.ClosingQueue.Queue.Count; i++)
            {
                ClosingQueueEntry q = InsanityBot.TicketDaemon.ClosingQueue.Queue[i];

                if(q.CloseDate < compareAgainst)
                {
                    try
                    {
                        await InsanityBot.TicketDaemon.DeleteTicket(
                            InsanityBot.TicketDaemon.Tickets.First(xm => xm.Value.DiscordChannelId == q.ChannelId).Value);
                        InsanityBot.TicketDaemon.ClosingQueue.Queue.Remove(q);
                    }
                    catch(Exception ex)
                    {
                        InsanityBot.Client.Logger.LogError(new EventId(3002, "TicketClose"),
                            $"Failed to fetch ticket from the daemon. Error:\n{ex}: {ex.Message}\n\n{ex.StackTrace}");

                        DiscordChannel channel = InsanityBot.HomeGuild.GetChannel(q.ChannelId);

                        DiscordEmbedBuilder error = new()
                        {
                            Description = "Failed to delete the ticket. This could be an InsanityBot or a Discord API issue." +
                                "\nPlease contact InsanityBot development at once and provide us with the console log.",
                            Color = DiscordColor.Red
                        };

                        await channel.SendMessageAsync(error.Build());

                        InsanityBot.TicketDaemon.ClosingQueue.Queue.Remove(q);
                    }
                }
                else
                {
                    continue;
                }
            }

            this.Countdown.Start();
        }
    }
}
