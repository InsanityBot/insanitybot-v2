using System;

namespace InsanityBot.Utility.Timers
{
    public delegate void TimerExpiredDelegate(String sender, Guid Id);

    public delegate void TimedActionCompleteEventHandler();

    public delegate void TimedActionStartEventHandler();
}