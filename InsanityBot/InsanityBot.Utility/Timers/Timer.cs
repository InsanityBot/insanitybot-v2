using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        private static async Task CallExpiredEvent(String Identifier, Guid guid)
        {
            TimerExpiredEvent?.Invoke(Identifier, guid);
        }

        public async Task<Boolean> CheckExpiry()
        {
            if (DateTime.UtcNow.Subtract(Expiry) <= TimeSpan.Zero)
            {
                _ = CallExpiredEvent(this.Identifier, this.Guid);
                return true;
            }
            return false;
        }

        public Timer(DateTime Expiry, String Identifier)
        {
            this.Expiry = Expiry;
            this.Identifier = Identifier;
            this.Guid = Guid.NewGuid();
        }
    }
}
