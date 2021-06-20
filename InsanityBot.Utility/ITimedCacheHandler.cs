using System;
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
