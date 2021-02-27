using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Utility.Exceptions
{
    public class InvalidDatafixerRegistryEntryException : Exception
    {
        public Type Datafixable { get; set; }
        public Type Datafixer { get; set; }

        public InvalidDatafixerRegistryEntryException(Type Datafixable, Type Datafixer, String Message) : base(Message)
        {
            this.Datafixable = Datafixable;
            this.Datafixer = Datafixer;
        }
    }
}
