using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Commands;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace InsanityBot.Tickets.Commands
{
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class CloseTicketCommand : BaseCommandModule
    {
        private Timer timer;
        private CommandContext ctx;

        [Command("close")]
        public async Task CloseTicket(CommandContext ctx,
            [RemainingText]
            String time = "30s")
        {
            TimeSpan closeTime = time.ParseTimeSpan();
            this.ctx = ctx;

            timer = new()
            {
                AutoReset = false,
                Interval = closeTime.TotalMilliseconds
            };

            timer.Start();

            timer.Elapsed += FinalCloseTicket;

            DiscordEmbedBuilder embedBuilder = new()
            {
                Description = InsanityBot.LanguageConfig["insanitybot.tickets.close"].ReplaceValues(ctx, ctx.Channel, closeTime),
                Color = DiscordColor.SpringGreen
            };

            await ctx.Channel.SendMessageAsync(embedBuilder.Build());
        }

        private async void FinalCloseTicket(Object sender, ElapsedEventArgs e)
        {
            try
            {
                await InsanityBot.TicketDaemon.DeleteTicket(
                    InsanityBot.TicketDaemon.Tickets.First(xm => xm.Value.DiscordChannelId == ctx.Channel.Id).Value);
            }
            catch(Exception ex)
            {
                await ctx.Channel.DeleteAsync("Ticket closed");
                InsanityBot.Client.Logger.LogError(new EventId(3002, "TicketClose"),
                    $"Failed to fetch ticket from the daemon. Error:\n{ex}: {ex.Message}\n\n{ex.StackTrace}");
            }
        }
    }
}
