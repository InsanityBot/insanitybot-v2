using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Timers
{
    public class Timer
    {
        public DateTime Expiry { get; set; }
        public String Identifier { get; set; }
        public Guid Guid { get; set; }

        public static event TimerExpiredDelegate TimerExpiredEvent;

        public async Task CheckExpiry()
        {
            if (DateTime.UtcNow.Subtract(Expiry) <= TimeSpan.Zero)
                await TimerExpiredEvent(this);
        }

        public Timer(DateTime Expiry, String Identifier)
        {
            this.Expiry = Expiry;
            this.Identifier = Identifier;
            this.Guid = Guid.NewGuid();
        }
    }
}
