using System;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Timers
{
    public delegate Task TimerExpiredDelegate(String sender, Guid Id);
}