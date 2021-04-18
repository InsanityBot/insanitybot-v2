using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace InsanityBot.Tickets
{
    public record TicketPreset
    {
        public UInt64 Category { get; set; }
        public String NameFormat { get; set; }
        public String Topic { get; set; }
        public TicketAccess AccessRules { get; set; }
        public DiscordMessage[] CreationMessages { get; set; }
        public DiscordEmbed[] CreationEmbeds { get; set; }
    }
}
