using System;
using System.Collections.Generic;
using System.Text;

using InsanityBot.Utility.Reference;

namespace InsanityBot.Utility
{
    public interface IConfigBuilder<ConfigType, ConfigBuilder> : IConfigBuilder
        where ConfigType : IConfiguration where ConfigBuilder : IConfigBuilder
    {
        public ConfigType Config { get; set; }
        public ConfigBuilder AddConfigEntry<T>(String Identifier, T DefaultValue);
        public ConfigBuilder RemoveConfigEntry(String Identifier);
    }
}
