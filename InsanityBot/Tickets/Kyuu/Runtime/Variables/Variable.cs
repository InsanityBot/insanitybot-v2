using InsanityBot.Tickets.Kyuu.Exceptions;
using InsanityBot.Tickets.Kyuu.Runtime.Variables.Datatypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Runtime.Variables
{
    public sealed class Variable
    {
        public VariableMutability Mutability { get; private set; }
        public VariableDatatype Datatype { get; private set; }
        public String Identifier { get; private set; }
        public Object Data { get; private set; }

        public Variable(VariableMutability mutability, VariableDatatype datatype, String identifier, Object data)
        {
            this.Mutability = mutability;
            this.Datatype = datatype;
            this.Identifier = identifier;
            this.Data = data;
        }

        public Boolean TryChangeData(Object data)
        {
            if (Mutability == VariableMutability.Constant)
                return false;

            if (AssociatedTypes.AssociationsInverse[data.GetType()] != Datatype)
                return false;

            this.Data = data;
            return true;
        }

        public void ChangeData(Object data)
        {
            if (Mutability == VariableMutability.Constant)
                throw new InvalidVariableAccessException("Cannot access constant variable", Identifier);

            if (AssociatedTypes.AssociationsInverse[data.GetType()] != Datatype)
                throw new InvalidTypeException("Cannot assign to variable of a different type",
                    AssociatedTypes.Associations[Datatype], data.GetType());

            this.Data = data;
        }
    }
}
