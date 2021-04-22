using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Tickets.Daemon;

using Microsoft.Extensions.Logging;

using static System.Convert;

namespace InsanityBot.Tickets.Kyuu
{
    public static class KyuuLoader
    {
        public static void Load()
        {
#if DEBUG
            InsanityBot.Client.Logger.LogInformation(new EventId(2100, "KyuuLoader"), "Loading Kyuu Engine");
#endif
            ActiveEngine = new(
                ToByte(TicketDaemon.Configuration["kyuu.threads"]),
                ToByte(TicketDaemon.Configuration["kyuu.queue_width"]),
                (Int64)(TicketDaemon.Configuration["kyuu.max_queue"]) == -1 ? Int32.MaxValue : ToInt32(TicketDaemon.Configuration["kyuu.max_queue"]),
                ToInt16(TicketDaemon.Configuration["kyuu.queue_interval"]));
        }

        public static Kyuu ActiveEngine { get; private set; }
    }
}
