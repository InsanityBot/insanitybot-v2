namespace InsanityBot.Tickets.Metrics;
using Emzi0767.Utilities;

internal interface IMetric<out Measure, in EventArgs>
	where EventArgs : AsyncEventArgs
{
	public Measure Result { get; }
}