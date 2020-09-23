using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility
{
    interface IConfigSerializer
    {
        public IConfiguration Deserialize();
        public void Serialize(IConfiguration Config);
    }
}
