namespace InsanityBot.Utility;
using System;
using System.Timers;

public interface ITimedCacheHandler
{
	public Timer Timer { get; set; }
	public void InitializeTimer();
	public void OnTimerElapsed(Object sender, ElapsedEventArgs e);
}