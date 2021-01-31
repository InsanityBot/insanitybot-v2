using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Datafixers.Reference
{
    public enum DatafixerLoadingResult
    {
        Success,
        CouldNotLoad,
        CouldNotInstantiate,
        ExceededTime //used if the datafixer just doesnt want to return a load state
    }
}
