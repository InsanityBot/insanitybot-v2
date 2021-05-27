using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace InsanityBot.Tickets
{
    public class TicketDaemon
    {
        internal delegate void KyuuTaskCreatedEventHandler();
        internal static event KyuuTaskCreatedEventHandler KyuuTaskCreatedEvent;

        private List<DiscordTicket> Tickets { get; set; }
        private Dictionary<Guid, DiscordTicketData> AdditionalData { get; set; }

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

            Configuration = new TicketConfigurationManager().Deserialize("./config/ticket.json");
        }

        public DiscordTicket GetDiscordTicket(UInt64 Id)
        {
            return (from v in Tickets
                    where v.DiscordChannelId == Id
                    select v).ToList().First();
        }

        public DiscordTicketData GetTicketData(UInt64 Id)
        {
            return (from v in AdditionalData
                    where v.Key == GetDiscordTicket(Id).TicketGuid
                    select v).ToList().First().Value;
        }

        public void SaveAll()
        {
            if(Tickets.Count > 0)
            {
                if (!File.Exists("./cache/tickets/tickets.json"))
                    File.Create("./cache/tickets/tickets.json").Close();

                StreamWriter writer = new("./cache/tickets/tickets.json");
                writer.Write(JsonConvert.SerializeObject(Tickets));
                writer.Close();
            }

            if(AdditionalData.Count > 0)
            {
                if (!File.Exists("./cache/tickets/data.json"))
                    File.Create("./cache/tickets/data.json").Close();

                StreamWriter writer = new("./cache/tickets/data.json");
                writer.Write(JsonConvert.SerializeObject(AdditionalData));
                writer.Close();
            }
        }

        ~TicketDaemon()
        {
            SaveAll();
        }
    }
}
