namespace InsanityBot.Tickets.Commands;
using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Commands;
using global::InsanityBot.Core.Attributes;

[ModuleLifespan(ModuleLifespan.Transient)]
public class CloseTicketCommand : BaseCommandModule
{

	[Command("close")]
	[RequirePermission("insanitybot.tickets.close")]
	public async Task CloseTicket(CommandContext ctx,
		[RemainingText]
			String args = "30s")
	{
		String time = args;
		Boolean forced = false;

		if(args.StartsWith("--force"))
		{
			time = args[7..];
			forced = true;
		}
		if(args.StartsWith("-f"))
		{
			time = args[2..];
			forced = true;
		}

		TimeSpan closeTime = time.ParseTimeSpan();

		if(!forced)
		{
			InsanityBot.TicketDaemon.ClosingQueue.AddToQueue(ctx.Channel.Id, closeTime);
		}
		else
		{
			InsanityBot.TicketDaemon.ClosingQueue.AddToQueue(ctx.Channel.Id, closeTime, false);
		}

		DiscordEmbedBuilder embedBuilder = InsanityBot.Embeds["insanitybot.tickets.close"]
			.WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.close"].ReplaceValues(ctx, ctx.Channel, closeTime));

		DiscordEmbedBuilder logBuilder = InsanityBot.Embeds["insanitybot.ticketlog.close_queue"]
			.AddField("Channel", ctx.Channel.Name, true)
			.AddField("Time", closeTime.ToString(), true);

		await InsanityBot.MessageLogger.LogMessage(logBuilder.Build(), ctx);

		await ctx.Channel?.SendMessageAsync(embedBuilder.Build());
	}
}