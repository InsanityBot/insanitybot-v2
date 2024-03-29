﻿namespace InsanityBot.Tickets.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Utility.Permissions;

public class AddUserCommand : BaseCommandModule
{
	[Command("add")]
	[RequirePermission("insanitybot.tickets.add")]
	public async Task AddUser(CommandContext ctx,
		params DiscordMember[] users)
	{
		IEnumerable<KeyValuePair<Guid, DiscordTicket>> x = from t in InsanityBot.TicketDaemon.Tickets
														   where t.Value.DiscordChannelId == ctx.Channel?.Id
														   select t;

		KeyValuePair<Guid, DiscordTicket> y = x.First();

		if(!x.Any())
		{
			DiscordEmbedBuilder error = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.add_user.not_a_ticket_channel"]
					.ReplaceValues(ctx, ctx.Channel));

			_ = ctx.Channel?.SendMessageAsync(error.Build());
			return;
		}

		foreach(DiscordMember v in users)
		{
			_ = ctx.Channel?.AddOverwriteAsync(v, Permissions.AccessChannels);

			DiscordTicket z = InsanityBot.TicketDaemon.Tickets[y.Key];

			if(z.AddedUsers == null)
			{
				z.AddedUsers = new List<UInt64>();
			}

			z.AddedUsers.Add(v.Id);
			InsanityBot.TicketDaemon.Tickets[y.Key] = z;

			if(TicketDaemon.Configuration.Value<Boolean>("insanitybot.tickets.ghost_mention_added_members"))
			{
				DiscordMessage msg = await ctx.Channel?.SendMessageAsync(v.Mention);
				_ = msg.DeleteAsync();
			}
		}
	}

	public async Task AddRole(CommandContext ctx,
		params DiscordRole[] roles)
	{
		if(!ctx.Member.HasPermission("insanitybot.tickets.add"))
		{
			await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
			return;
		}

		IEnumerable<KeyValuePair<Guid, DiscordTicket>> x = from t in InsanityBot.TicketDaemon.Tickets
														   where t.Value.DiscordChannelId == ctx.Channel?.Id
														   select t;

		if(!x.Any())
		{
			DiscordEmbedBuilder error = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.add_user.not_a_ticket_channel"]
					.ReplaceValues(ctx, ctx.Channel));

			_ = ctx.Channel?.SendMessageAsync(error.Build());
			return;
		}

		foreach(DiscordRole v in roles)
		{
			_ = ctx.Channel?.AddOverwriteAsync(v, Permissions.AccessChannels);

			if(TicketDaemon.Configuration.Value<Boolean>("insanitybot.tickets.ghost_mention_added_members"))
			{
				DiscordMessage msg = await ctx.Channel?.SendMessageAsync(v.Mention);
				_ = msg.DeleteAsync();
			}
		}
	}
}