using System;
using System.Collections.Generic;

namespace InsanityBot.Tickets.Placeholders
{
	internal static class PlaceholderList
	{
		public static Dictionary<String, Func<DiscordTicket, String>> Placeholders = new()
		{
			{
				"user.username",
				xm =>
				{
					return InsanityBot.HomeGuild.GetMemberAsync(xm.Creator).GetAwaiter().GetResult().Username;
				}
			},
			{
				"user.nickname",
				xm =>
				{
					return InsanityBot.HomeGuild.GetMemberAsync(xm.Creator).GetAwaiter().GetResult().Nickname;
				}
			},
			{
				"user.id",
				xm =>
				{
					return xm.Creator.ToString();
				}
			},
			{
				"user.discriminator",
				xm =>
				{
					return InsanityBot.HomeGuild.GetMemberAsync(xm.Creator).GetAwaiter().GetResult().Discriminator;
				}
			},
			{
				"user.mention",
				xm =>
				{
					return InsanityBot.HomeGuild.GetMemberAsync(xm.Creator).GetAwaiter().GetResult().Mention;
				}
			},
			{
				"guild.name",
				xm =>
				{
					return InsanityBot.HomeGuild.Name;
				}
			},
			{
				"guild.id",
				xm =>
				{
					return InsanityBot.HomeGuild.Id.ToString();
				}
			},
			{

				"global.number",
				xm =>
				{
					return TicketDaemon.TicketCount.ToString();
				}
			},
			{
				"global.guid",
				xm =>
				{
					return Guid.NewGuid().ToString();
				}
			},
			{
				"global.random",
				xm =>
				{
					return TicketDaemon.RandomTicketName;
				}
			},
			{
				"ticket.name",
				xm =>
				{
					return xm.DiscordChannelId.ToString();
				}
			},
			{
				"ticket.mention",
				xm =>
				{
					return InsanityBot.HomeGuild.GetChannel(xm.DiscordChannelId).Mention;
				}
			}
		};
	}
}
