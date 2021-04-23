using System;
using System.Runtime.CompilerServices;

namespace InsanityBot.Utility.Timers
{
    public class Timer
    {
        public DateTime Expiry { get; set; }
        public String Identifier { get; set; }
        public Guid Guid { get; set; }

        public static event TimerExpiredDelegate TimerExpiredEvent;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CallExpiredEvent(String Identifier, Guid guid) => TimerExpiredEvent?.Invoke(Identifier, guid);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Boolean CheckExpiry()
        {
            if (DateTime.UtcNow > Expiry.ToUniversalTime())
            {
                CallExpiredEvent(this.Identifier, this.Guid);
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
