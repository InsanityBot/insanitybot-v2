using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Exceptions;
using InsanityBot.Utility.Reference;

namespace InsanityBot.Utility.Datafixers.Reference
{
    public struct DatafixerRegistryEntry
    {
        public Guid DatafixerGuid { get; init; }

        // this is just an instance to skip some slow reflection nonsense, technically passing a type would work too
        // try not leaking any ram in your IDatafixer implementation since the instance doesnt get destroyed at runtime
        public IDatafixer Datafixer { get; init; }
        public Type DatafixerTarget { get; init; }
        public Boolean BreakingChange { get; init; }
        public UInt32 DatafixerId { get; init; } // please dont attempt to datafix IDatafixable's you didnt implement, thatll just cause confusion

        public SortedDatafixerRegistryEntry ToSortedDatafixerRegistryEntry()
        {
            return new SortedDatafixerRegistryEntry
            {
                DatafixerGuid = this.DatafixerGuid,
                Datafixer = this.Datafixer,
                BreakingChange = this.BreakingChange,
                DatafixerId = this.DatafixerId
            };
        }
    }
}
