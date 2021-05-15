using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Exceptions
{
    public class InvalidVariableAccessException : Exception
    {
        public String VariableName { get; }

        public InvalidVariableAccessException(String Message, String VariableName) : base(Message)
            => this.VariableName = VariableName;
    }
}
