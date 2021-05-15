using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Kyuu.Runtime.Variables.Datatypes
{
    internal static class AssociatedTypes
    {
        internal static Dictionary<VariableDatatype, Type> Associations = new()
        {
            { VariableDatatype.DiscordEmbed, typeof(MockDiscordEmbed) },
            { VariableDatatype.DiscordMessage, typeof(MockDiscordMessage) },
            { VariableDatatype.Guid, typeof(Guid) },
            { VariableDatatype.boolean, typeof(Boolean) },
            { VariableDatatype.int16, typeof(Int16) },
            { VariableDatatype.int32, typeof(Int32) },
            { VariableDatatype.int64, typeof(Int64) },
            { VariableDatatype.uint16, typeof(UInt16) },
            { VariableDatatype.uint32, typeof(UInt32) },
            { VariableDatatype.uint64, typeof(UInt64) }
        };

        internal static Dictionary<Type, VariableDatatype> AssociationsInverse = new()
        {
            { typeof(MockDiscordEmbed), VariableDatatype.DiscordEmbed },
            { typeof(MockDiscordMessage), VariableDatatype.DiscordMessage },
            { typeof(Guid), VariableDatatype.Guid },
            { typeof(Boolean), VariableDatatype.boolean },
            { typeof(Int16), VariableDatatype.int16 },
            { typeof(Int32), VariableDatatype.int32 },
            { typeof(Int64), VariableDatatype.int64 },
            { typeof(UInt16), VariableDatatype.uint16 },
            { typeof(UInt32), VariableDatatype.uint32 },
            { typeof(UInt64), VariableDatatype.uint64 }
        };
    }
}
