namespace InsanityBot.Tickets.Placeholders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Emzi0767;

public static class Parser
{
	public static Task<String> ParseTicketPlaceholders(String value, Guid ticket)
	{
		String tempValue = value;

		if(!tempValue.Contains('{'))
		{
			return Task.FromResult(tempValue);
		}

		foreach(KeyValuePair<String, Func<DiscordTicket, String>> v in PlaceholderList.Placeholders)
		{
			if(v.Key.StartsWith("ticket."))
			{
				continue;
			}
			tempValue = tempValue.Replace($"{{{v.Key}}}", v.Value(InsanityBot.TicketDaemon.Tickets[ticket]));
		}

		value = tempValue;

		value += "\u0003"; // append EOT character so we know where to break
		Char currentCharacter = ' ';
		String placeholderName = " ";
		StringBuilder builder = new();
		Boolean buildingName = false, buildingLength = false;

		for(Int32 i = 0; ; i++)
		{
			Char lastCharacter = currentCharacter;
			currentCharacter = value[i];

			if(currentCharacter == '\u0003')
			{
				break;
			}

			if(currentCharacter == '{' && lastCharacter != '\\')
			{
				buildingName = true;
			}

			if(currentCharacter == ':' && lastCharacter != '\\')
			{
				buildingName = false;
				buildingLength = true;
				placeholderName = builder.ToString();
				builder.Clear();
			}

			if(currentCharacter == '}' && lastCharacter != '\\')
			{
				buildingLength = false;
				Int32 replaceLength = Convert.ToInt32(builder.ToString());
				builder.Clear();

				if(placeholderName.StartsWith("ticket."))
				{
					continue;
				}

				tempValue = tempValue.Replace($"{{{placeholderName}:{replaceLength}}}", PlaceholderList.Placeholders[placeholderName].
						 Invoke(InsanityBot.TicketDaemon.Tickets[ticket])[..replaceLength]);
			}

			if(buildingName && (currentCharacter.IsBasicLetter() || currentCharacter == '.'))
			{
				builder.Append(currentCharacter);
			}

			if(buildingLength && currentCharacter.IsBasicDigit())
			{
				builder.Append(currentCharacter);
			}

			continue;
		}

		return Task.FromResult(tempValue);
	}
}