using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility
{
    public interface IConfiguration<T>
    {
        public String DataVersion { get; set; }

        // if anyone knows how to make this fully typesafe while keeping compatibility for String, Boolean, all those ints and IEnumerable, submit a PR
        public Dictionary<String, T> Configuration { get; set; }
    }
}
