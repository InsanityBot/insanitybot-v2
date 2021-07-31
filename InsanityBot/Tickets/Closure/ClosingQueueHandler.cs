﻿using DSharpPlus.Entities;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
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
            Countdown = new()
            {
                Interval = 2000,
                AutoReset = false
            };
            Countdown.Elapsed += CountdownElapsed;

            Countdown.Start();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CountdownElapsed(Object sender, ElapsedEventArgs e) 
        {
            _ = CountdownElapsedAsync();
        }

        private async Task CountdownElapsedAsync()
        {
            DateTime compareAgainst = DateTime.Now;

            for(int i = 0; i < InsanityBot.TicketDaemon.ClosingQueue.Queue.Count; i++)
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

            Countdown.Start();
        }
    }
}
