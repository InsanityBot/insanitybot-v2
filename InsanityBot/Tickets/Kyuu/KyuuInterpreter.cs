using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Tickets.Kyuu.Preprocessor;

namespace InsanityBot.Tickets.Kyuu
{
    public class KyuuInterpreter
    {
        public async Task InterpretScript(String filename, DiscordMessage message)
        {
            StreamReader reader = new(filename);

            #region Preprocessor
            KyuuPreprocessorContext context = new()
            {
                Instruction = "placeholder",
                Message = message,
                Ticket = InsanityBot.TicketDaemon.GetDiscordTicket(message.ChannelId),
                TicketData = InsanityBot.TicketDaemon.GetTicketData(message.ChannelId)
            };
            String line;

            do
            {
                line = reader.ReadLine();

                KyuuPreprocessorContext activeContext = context with { Instruction = line };

                if (!await KyuuPreprocessorDirective.Directives[line.Split(' ')[0].Remove(0, 1)].Task(activeContext))
                    return;
            } 
            while (line.StartsWith('#'));
            #endregion

            #region Constants
            reader.BaseStream.Position = 0; // read from start
            do
            {

            }
            while (reader.EndOfStream);
            #endregion
        }
    }
}
