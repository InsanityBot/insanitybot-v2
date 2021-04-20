using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace InsanityBot.Tickets.Daemon
{
    public class TicketDaemon
    {
        List<DiscordTicket> Tickets { get; set; }
        Dictionary<Guid, DiscordTicketData> AdditionalData { get; set; }

        public TicketDaemon()
        {
            if(!File.Exists("./cache/tickets/presets/default.json"))
            {
                InsanityBot.Client.Logger.LogCritical(new EventId(2000, "TicketDaemon"), "Could not find default ticket preset, aborting...");
                Process.GetCurrentProcess().Kill();
            }

            if (!File.Exists("./cache/tickets/tickets.json"))
                Tickets = new();
            else
                Tickets = JsonConvert.DeserializeObject<List<DiscordTicket>>(File.ReadAllText("./cache/tickets/tickets.json"));

            if (!File.Exists("./cache/tickets/data.json"))
                AdditionalData = new();
            else
                AdditionalData = JsonConvert.DeserializeObject<Dictionary<Guid, DiscordTicketData>>(
                    File.ReadAllText("./cache/tickets/data.json"));
        }
    }
}
