using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public record KyuuPreprocessorContext
    {
        public DiscordMessage Message { get; set; }
        public String Instruction { get; set; }
        public DiscordTicketData TicketData { get; set; }
        public DiscordTicket Ticket { get; set; }
    }
}
