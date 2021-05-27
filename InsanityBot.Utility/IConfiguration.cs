using InsanityBot.Utility.Datafixers;

using System;
using System.Collections.Generic;

namespace InsanityBot.Utility
{
    public interface IConfiguration<T> : IDatafixable
    {
        public Dictionary<String, T> Configuration { get; set; }
    }
}
