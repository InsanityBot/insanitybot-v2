namespace InsanityBot.Tickets.Commands.Admin;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using global::InsanityBot.Core.Attributes;

[Group("clearticketcache")]
[Aliases("clear-ticket-cache")]
public class ClearTicketCache : BaseCommandModule
{
	[RequirePrefixes("admin.")]
	[Command("full")]
	[Aliases("entire", "complete")]
	[RequireAdminPermission("insanitybot.admin.ticket.clear_cache")]
	public Task ClearFullCache(CommandContext ctx)
	{
		File.Delete("./cache/tickets/tickets.json");
		File.Delete("./cache/tickets/closequeue.json");

		InsanityBot.TicketDaemon = new();
		InsanityBot.TicketDaemon.CommandHandler.Load();

		TicketDaemonState state = new();
		state.RestoreDaemonState(ref InsanityBot._ticketDaemon);

		InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.RouteCustomCommand;
		InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.ClosingQueue.HandleCancellation;

		return Task.CompletedTask;
	}

	[RequirePrefixes("admin.")]
	[Command("ticket")]
	[Aliases("tickets")]
	[RequireAdminPermission("insanitybot.admin.ticket.clear_cache")]
	public Task ClearTickets(CommandContext ctx)
	{
		File.Delete("./cache/tickets/tickets.json");

		InsanityBot.TicketDaemon = new();
		InsanityBot.TicketDaemon.CommandHandler.Load();

		TicketDaemonState state = new();
		state.RestoreDaemonState(ref InsanityBot._ticketDaemon);

		InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.RouteCustomCommand;
		InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.ClosingQueue.HandleCancellation;

		return Task.CompletedTask;
	}

	[RequirePrefixes("admin.")]
	[Command("closequeue")]
	[Aliases("close")]
	[RequireAdminPermission("insanitybot.admin.ticket.clear_cache")]
	public Task ClearClosequeue(CommandContext ctx)
	{
		File.Delete("./cache/tickets/closequeue.json");

		InsanityBot.TicketDaemon = new();
		InsanityBot.TicketDaemon.CommandHandler.Load();

		TicketDaemonState state = new();
		state.RestoreDaemonState(ref InsanityBot._ticketDaemon);

		InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.RouteCustomCommand;
		InsanityBot.Client.MessageCreated += InsanityBot.TicketDaemon.ClosingQueue.HandleCancellation;

		return Task.CompletedTask;
	}
}