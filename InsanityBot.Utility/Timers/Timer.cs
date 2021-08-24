using System;
using System.Runtime.CompilerServices;

namespace InsanityBot.Utility.Timers
{
    public class Timer
    {
        public DateTimeOffset Expiry { get; set; }
        public String Identifier { get; set; }
        public Guid Guid { get; set; }

        public static event TimerExpiredDelegate TimerExpiredEvent;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CallExpiredEvent(String Identifier, Guid guid) => TimerExpiredEvent?.Invoke(Identifier, guid);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Boolean CheckExpiry()
        {
            if(DateTimeOffset.UtcNow > this.Expiry.ToUniversalTime())
            {
                CallExpiredEvent(this.Identifier, this.Guid);
                return true;
            }
            return false;
        }

        public Timer(DateTimeOffset Expiry, String Identifier)
        {
            this.Expiry = Expiry;
            this.Identifier = Identifier;
            this.Guid = Guid.NewGuid();
        }
    }
}
