using InsanityBot.Utility.Reference;

using System;

namespace InsanityBot.Utility.Datafixers.Reference
{
    public struct SortedDatafixerRegistryEntry
    {
        public Guid DatafixerGuid { get; init; }
        public IDatafixer Datafixer { get; init; }
        public Boolean BreakingChange { get; init; }
        public UInt32 DatafixerId { get; init; }
    }
}
