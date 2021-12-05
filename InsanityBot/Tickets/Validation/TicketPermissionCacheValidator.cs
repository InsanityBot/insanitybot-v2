namespace InsanityBot.Tickets.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

public class TicketPermissionCacheValidator
{
	public Task Validate(DiscordClient sender, GuildDownloadCompletedEventArgs args)
	{
		_ = Task.Run(() =>
		{
			if(!args.Guilds.ContainsKey(InsanityBot.HomeGuild.Id))
			{
				return;
			}

			List<UInt64> cacheIds = InsanityBot.TicketDaemon.Tickets.Select(xm => xm.Value.DiscordChannelId).ToList();

			foreach(UInt64 v in cacheIds)
			{
				DiscordTicket ticket = InsanityBot.TicketDaemon.Tickets
					.Where(xm => xm.Value.DiscordChannelId == v)
					.Select(xm => xm.Value)
					.First();

				IEnumerable<UInt64> overrides = from d in InsanityBot.HomeGuild.GetChannel(v).PermissionOverwrites
												where d.Allowed == Permissions.AccessChannels
												where d.Type == OverwriteType.Member
												select d.Id;

				if(ticket.AddedUsers.OrderBy(xm => xm).SequenceEqual(overrides.OrderBy(xm => xm)))
				{
					continue;
				}

				foreach(UInt64 y in ticket.AddedUsers)
				{
					if(!overrides.Contains(y))
					{
						ticket.AddedUsers.Remove(v);
					}
				}
				foreach(UInt64 z in overrides)
				{
					if(!ticket.AddedUsers.Contains(z))
					{
						ticket.AddedUsers.Add(z);
					}
				}

				InsanityBot.TicketDaemon.Tickets[ticket.TicketGuid] = ticket;
			}

			InsanityBot.TicketDaemon.SaveAll();

			return;
		});
		return Task.CompletedTask;
	}
}