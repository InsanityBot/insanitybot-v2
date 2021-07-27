using Emzi0767;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Placeholders
{
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
                tempValue = tempValue.Replace($"{{{v.Key}}}", v.Value(InsanityBot.TicketDaemon.Tickets[ticket]));
            }

            value += "\u0003"; // append EOT character so we know where to break
            Char currentCharacter = ' ', lastCharacter = ' ';
            Int32 replaceLength = 0;
            String placeholderName = " ";
            StringBuilder builder = new();
            Boolean buildingName = false, buildingLength = false;

            for(int i = 0; ; i++)
            {
                lastCharacter = currentCharacter;
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
                    replaceLength = Convert.ToInt32(builder.ToString());
                    builder.Clear();
                    goto REPLACE_VALUES;
                }

                if(buildingName && currentCharacter.IsBasicLetter())
                {
                    builder.Append(currentCharacter);
                }

                if(buildingLength && currentCharacter.IsBasicDigit())
                {
                    builder.Append(currentCharacter);
                }

            REPLACE_VALUES:
                tempValue = tempValue.Replace($"{{{placeholderName}:{replaceLength}}}", PlaceholderList.Placeholders[placeholderName].
                         Invoke(InsanityBot.TicketDaemon.Tickets[ticket]).Substring(0, replaceLength));
            }

            return Task.FromResult(tempValue);
        }
    }
}
