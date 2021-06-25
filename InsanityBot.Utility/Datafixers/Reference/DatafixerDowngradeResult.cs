using System;

namespace InsanityBot.Utility.Datafixers.Reference
{
    public enum DatafixerDowngradeResult : Int16
    {
        Success,
        PartialSuccess,
        Failure,
        AlreadyDowngraded
    }
}
