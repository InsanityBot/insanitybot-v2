using InsanityBot.Utility.Datafixers.Reference;

using System;
using System.Collections.Generic;

namespace InsanityBot.Utility.Converters
{
    internal static class SortedToRawDatafixerRegistryEntry
    {
        internal static DatafixerRegistryEntry ToUnsorted(this SortedDatafixerRegistryEntry entry, Type type) => new DatafixerRegistryEntry
        {
            BreakingChange = entry.BreakingChange,
            Datafixer = entry.Datafixer,
            DatafixerGuid = entry.DatafixerGuid,
            DatafixerId = entry.DatafixerId,
            DatafixerTarget = type
        };

        internal static IEnumerable<DatafixerRegistryEntry> ToUnsorted(this IEnumerable<SortedDatafixerRegistryEntry> entries, Type type)
        {
            List<DatafixerRegistryEntry> returnValue = new();
            foreach (SortedDatafixerRegistryEntry entry in entries)
            {
                returnValue.Add(new DatafixerRegistryEntry
                {
                    BreakingChange = entry.BreakingChange,
                    Datafixer = entry.Datafixer,
                    DatafixerGuid = entry.DatafixerGuid,
                    DatafixerId = entry.DatafixerId,
                    DatafixerTarget = type
                });
            }

            return returnValue;
        }
    }
}
