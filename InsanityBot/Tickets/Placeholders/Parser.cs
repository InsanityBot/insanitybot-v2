using System;
using System.Collections.Generic;
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

                if(!tempValue.Contains($"{{{v.Key}:"))
                {
                    continue;
                }

                // unoptimized. feel free to improve
                List<Int32> indices = new();
                List<Byte> lengthValues = new();

                for(Int32 i = 0; i < tempValue.Length; i++)
                {
                    if(tempValue[i] != '{')
                    {
                        continue;
                    }

                    if(tempValue[i..(i + v.Key.Length + 1)] != $"{{{v.Key}:")
                    {
                        continue;
                    }

                    indices.Add(i);

                    Int32 closingBracketIndex = tempValue.Substring((i + v.Key.Length + 1), 3).IndexOf('}');
                    lengthValues.Add(Convert.ToByte(tempValue[(i + v.Key.Length + 1)..(closingBracketIndex + 1)]));
                }

                for(Byte b = 0; b < indices.Count; b++)
                {
                    tempValue = tempValue.Replace($"{{{v.Key}:{lengthValues[b]}}}",
                        $"{v.Value(InsanityBot.TicketDaemon.Tickets[ticket]).Substring(0, lengthValues[b])}");
                }
            }

            return Task.FromResult(tempValue);
        }
    }
}
