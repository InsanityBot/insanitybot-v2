using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Exceptions;

namespace InsanityBot.Utility.Datafixers.Reference
{
    public struct DatafixerRegistryEntry
    {
        public Type Datafixable { get; set; }
        public Type Datafixer { get; set; }

        public DatafixerRegistryEntry(Type Datafixable, Type Datafixer)
        {
            if (Datafixable.GetInterfaces().Contains(typeof(IDatafixable)))
                this.Datafixable = Datafixable;
            else
                throw new InvalidDatafixerRegistryEntryException(Datafixable, Datafixer,
                    "Error creating datafixer registry entry: the passed datafixable does not implement InsanityBot.Utility.Datafixers.IDatafixable");

            if (Datafixer.GetInterfaces().Contains(typeof(IDatafixer)))
                this.Datafixer = Datafixer;
            else
                throw new InvalidDatafixerRegistryEntryException(Datafixable, Datafixer,
                    "Error creating datafixer registry entry: the passed datafixer does not implement InsanityBot.Utility.Datafixers.IDatafixer");
        }
    }
}
