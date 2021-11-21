namespace InsanityBot.Utility.Converters;
using System;
using System.Collections.Generic;

using InsanityBot.Utility.Datafixers.Reference;

internal static class SortedToRawDatafixerRegistryEntry
{
	internal static DatafixerRegistryEntry ToUnsorted(this SortedDatafixerRegistryEntry entry, Type type) => new()
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
		foreach(SortedDatafixerRegistryEntry entry in entries)
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