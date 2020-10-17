using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility
{
    public interface IConfiguration<T>
    {
        public String DataVersion { get; set; }

        public Dictionary<String, T> Configuration { get; set; }
    }
}
