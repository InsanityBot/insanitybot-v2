using InsanityBot.Utility.Datafixers;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;

namespace InsanityBot.Utility
{
    public interface IConfiguration : IDatafixable
    {
        public JObject Configuration { get; set; }
    }
}
