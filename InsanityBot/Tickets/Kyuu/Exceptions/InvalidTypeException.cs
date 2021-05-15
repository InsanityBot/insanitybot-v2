using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public Type RequiredType { get; }
        public Type GivenType { get; }

        public InvalidTypeException(String Message, Type RequiredType, Type GivenType) : base(Message)
        {
            this.RequiredType = RequiredType;
            this.GivenType = GivenType;
        }
    }
}
