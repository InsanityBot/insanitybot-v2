namespace InsanityBot.Tickets.Validation;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

public class ChannelDeleteValidator
{
	public Task Validate(DiscordClient client, ChannelDeleteEventArgs args)
	{
		_ = Task.Run(() =>
		{
			if(!InsanityBot.TicketDaemon.Tickets.Any(xm => xm.Value.DiscordChannelId == args.Channel.Id))
			{
				return;
			}

			InsanityBot.TicketDaemon.RemoveTicket(args.Channel.Id);
		});
		return Task.CompletedTask;
	}
}