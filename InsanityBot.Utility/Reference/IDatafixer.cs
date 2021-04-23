using System;

using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Utility.Reference
{
    public interface IDatafixer
    {
        public DatafixerLoadingResult Load() => DatafixerLoadingResult.Success;

        public String NewDataVersion { get; }
        public String OldDataVersion { get; }
        public UInt32 DatafixerId { get; }
        public Boolean BreakingChange { get; }
    }
}
