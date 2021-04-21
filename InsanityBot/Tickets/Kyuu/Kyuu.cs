using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu
{
    public class Kyuu
    {
        private Byte Threads { get; set; }
        private Byte QueueWidth { get; set; }
        private Int16 QueueInterval { get; set; }
        private Int32 MaxQueue { get; set; }

        public Kyuu(Byte threads, Byte queueWidth, Int32 maxQueue, Int16 queueInterval)
        {
            this.Threads = threads;
            this.QueueWidth = queueWidth;
            this.MaxQueue = maxQueue;
            this.QueueInterval = queueInterval;
        }
    }
}
