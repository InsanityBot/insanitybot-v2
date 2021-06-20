using System;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Placeholders
{
	public static class Parser
	{
		public static Task<String> ParseTicketPlaceholders(String value, Guid ticket)
		{
			String tempValue = value;

			if (!tempValue.Contains('{'))
			{
				return Task.FromResult(tempValue);
			}

			foreach (System.Collections.Generic.KeyValuePair<String, Func<DiscordTicket, String>> v in PlaceholderList.Placeholders)
			{
				tempValue = tempValue.Replace($"{{{v.Key}}}", v.Value(InsanityBot.TicketDaemon.Tickets[ticket]));
			}

			return Task.FromResult(tempValue);
		}
	}
}
