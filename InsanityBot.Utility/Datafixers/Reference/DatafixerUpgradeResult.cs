namespace InsanityBot.Utility.Datafixers.Reference;
using System;

public enum DatafixerUpgradeResult : Int16
{
	Success,
	PartialSuccess,
	Failure,
	AlreadyUpgraded
}