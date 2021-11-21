namespace InsanityBot.Tickets.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

public class TicketCacheValidator
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
			List<UInt64> channelIds = args.Guilds[InsanityBot.HomeGuild.Id].Channels.Select(xm => xm.Key).ToList();

			foreach(UInt64 v in cacheIds)
			{
				if(!channelIds.Contains(v))
				{
					InsanityBot.TicketDaemon.RemoveTicket(v);
				}
			}

			InsanityBot.TicketDaemon.SaveAll();

			return;
		});
		return Task.CompletedTask;
	}
}