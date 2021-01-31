using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Datafixers.Reference;

namespace InsanityBot.Utility.Datafixers
{
    internal class DatafixerRegistry
    {
        private readonly List<DatafixerRegistryEntry> Registry;

        public List<DatafixerRegistryEntry> GetDatafixers(Type Datafixable)
        {
            return (from v in Registry
                    where v.Datafixable == Datafixable
                    select v).ToList();
        }

        public void AddDatafixer(Type Datafixer, Type Datafixable)
        {
            Registry.Add(new DatafixerRegistryEntry(Datafixable, Datafixer));
        }

        public List<DatafixerRegistryEntry> GetRegistryEntries()
            => Registry;

        public DatafixerRegistry()
        {
            Registry = new List<DatafixerRegistryEntry>();
        }
    }
}
