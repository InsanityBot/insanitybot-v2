using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility
{
    interface IConfiguration
    {
        public String DataVersion { get; set; }
        public Dictionary<String, Object> Configuration { get; set; }
    }
}
