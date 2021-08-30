using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using InsanityBot.Utility.Permissions;

namespace InsanityBot.Tickets.Commands.Admin
{
    [Group("clearticketcache")]
    [Aliases("clear-ticket-cache")]
    public class ClearTicketCache : BaseCommandModule
    {
        [RequirePrefixes("admin.")]
        [Command("full")]
        [Aliases("entire", "complete")]
        public async Task ClearFullCache(CommandContext ctx)
        {
            if(!ctx.Member.HasPermission("insanitybot.admin.ticket.clear_cache"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                return;
            }

            File.Delete("./cache/tickets/tickets.json");
            File.Delete("./cache/tickets/closequeue.json");

            InsanityBot.TicketDaemon = new();
            InsanityBot.TicketDaemon.CommandHandler.Load();

            TicketDaemonState state = new();
            state.RestoreDaemonState(ref InsanityBot._ticketDaemon);

            InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.RouteCustomCommand;
            InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.ClosingQueue.HandleCancellation;
        }

        [RequirePrefixes("admin.")]
        [Command("ticket")]
        [Aliases("tickets")]
        public async Task ClearTickets(CommandContext ctx)
        {
            if(!ctx.Member.HasPermission("insanitybot.admin.ticket.clear_cache"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                return;
            }

            File.Delete("./cache/tickets/tickets.json");

            InsanityBot.TicketDaemon = new();
            InsanityBot.TicketDaemon.CommandHandler.Load();

            TicketDaemonState state = new();
            state.RestoreDaemonState(ref InsanityBot._ticketDaemon);

            InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.RouteCustomCommand;
            InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.ClosingQueue.HandleCancellation;
        }

        [RequirePrefixes("admin.")]
        [Command("closequeue")]
        [Aliases("close")]
        public async Task ClearClosequeue(CommandContext ctx)
        {
            if(!ctx.Member.HasPermission("insanitybot.admin.ticket.clear_cache"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                return;
            }

            File.Delete("./cache/tickets/closequeue.json");

            InsanityBot.TicketDaemon = new();
            InsanityBot.TicketDaemon.CommandHandler.Load();

            TicketDaemonState state = new();
            state.RestoreDaemonState(ref InsanityBot._ticketDaemon);

            InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.RouteCustomCommand;
            InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.ClosingQueue.HandleCancellation;
        }
    }
}
