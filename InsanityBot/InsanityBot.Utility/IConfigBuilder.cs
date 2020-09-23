using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility
{
    interface IConfigBuilder
    {
        protected IConfiguration Config { get; set; }
        public IConfigBuilder AddConfigEntry(String Identifier, Object DefaultValue);
        public IConfigBuilder RemoveConfigEntry(String Identifier);
    }
}
