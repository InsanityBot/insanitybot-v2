using System;

namespace InsanityBot.Utility.Timers
{
	public delegate void TimerExpiredDelegate(String sender, Guid Id);

	public delegate void UnmuteCompletedDelegate();
	public delegate void UnbanCompletedDelegate();

	public delegate void MuteStartingDelegate();
	public delegate void BanStartingDelegate();
}