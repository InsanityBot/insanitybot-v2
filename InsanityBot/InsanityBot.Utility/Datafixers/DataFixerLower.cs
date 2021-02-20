using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Utility.Datafixers
{
    public class DataFixerLower //code style: capital F intended
    {
        private readonly DatafixerRegistry Registry;

        public DataFixerLower(Byte registryMode)
        {
            Registry = new DatafixerRegistry(registryMode);
        }
        

    }
}
