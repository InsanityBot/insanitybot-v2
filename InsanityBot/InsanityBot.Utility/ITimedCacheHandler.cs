using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace InsanityBot.Utility
{
    public interface ITimedCacheHandler
    {
        public Timer Timer { get; set; }
        public void InitializeTimer();
        public void OnTimerElapsed(Object sender, ElapsedEventArgs e);
    }
}
