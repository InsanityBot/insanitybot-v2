using System;
using System.Collections.Generic;
using System.Text;

using InsanityBot.Utility.Datafixers;

namespace InsanityBot.Utility
{
    public interface IConfiguration<T> : IDatafixable
    {
        public Dictionary<String, T> Configuration { get; set; }
    }
}
