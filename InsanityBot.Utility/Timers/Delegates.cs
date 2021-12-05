namespace InsanityBot.Utility.Timers;
using System;

public delegate void TimerExpiredDelegate(String sender, Guid Id);

public delegate void TimedActionCompleteEventHandler();

public delegate void TimedActionStartEventHandler();