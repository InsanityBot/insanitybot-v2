using System;

namespace InsanityBot.Utility.Datafixers.Reference
{
    public enum DatafixerUpgradeResult : Int16
    {
        Success,
        PartialSuccess,
        Failure,
        AlreadyUpgraded
    }
}
