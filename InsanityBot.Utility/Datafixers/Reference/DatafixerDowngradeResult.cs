namespace InsanityBot.Utility.Datafixers.Reference;
using System;

public enum DatafixerDowngradeResult : Int16
{
	Success,
	PartialSuccess,
	Failure,
	AlreadyDowngraded
}