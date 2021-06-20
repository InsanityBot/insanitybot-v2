using System;

namespace InsanityBot.Utility.Datafixers.Reference
{
	public enum DatafixerLoadingResult : Int16
	{
		Success,
		CouldNotLoad,
		ExceededTime
	}
}
