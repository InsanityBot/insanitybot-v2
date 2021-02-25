using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Utility.Reference
{
    public interface IDatafixer
    {
        public static DatafixerLoadingResult Load()
        {
            return DatafixerLoadingResult.Success;
        }
    }
}
