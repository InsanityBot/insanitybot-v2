using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Tickets.Daemon.Config;
using InsanityBot.Tickets.Kyuu;
using InsanityBot.Tickets.Kyuu.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace InsanityBot.Tickets.Daemon
{
    public class TicketDaemon
    {
        internal delegate void KyuuTaskCreatedEventHandler();
        internal static event KyuuTaskCreatedEventHandler KyuuTaskCreatedEvent;

        private List<DiscordTicket> Tickets { get; set; }
        private Dictionary<Guid, DiscordTicketData> AdditionalData { get; set; }

        internal static List<KyuuTask> Tasks { get; private set; }
        internal static TicketConfiguration Configuration { get; set; }

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

            KyuuLoader.Load();

            Tasks = new();
            Configuration = new TicketConfigurationManager().Deserialize("./config/tickets.json");
        }
    }
}
