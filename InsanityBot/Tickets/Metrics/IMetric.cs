using Emzi0767.Utilities;

namespace InsanityBot.Tickets.Metrics
{
    internal interface IMetric<out Measure, in EventArgs>
        where EventArgs : AsyncEventArgs
    {
        public Measure Result { get; }
    }
}
