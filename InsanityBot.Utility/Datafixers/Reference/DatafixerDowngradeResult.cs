using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
