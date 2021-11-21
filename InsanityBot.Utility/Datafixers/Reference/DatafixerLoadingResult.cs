namespace InsanityBot.Utility.Datafixers.Reference;
using System;

public enum DatafixerLoadingResult : Int16
{
	Success,
	CouldNotLoad,
	ExceededTime
}