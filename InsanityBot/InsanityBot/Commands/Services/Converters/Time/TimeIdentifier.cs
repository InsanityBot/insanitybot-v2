using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Commands.Services.Converters.Time
{
    internal sealed class TimeIdentifier
    {
        internal UInt64 Ticks { get; init; }
        internal Char PrimaryIdentifier { get; init; }
        internal String[] SecondaryIdentifiers { get; init; }
        internal String FullName { get; init; }

        internal TimeIdentifier(UInt64 ticks, Char identifier, String name, params String[] secondaries) =>
            (Ticks, PrimaryIdentifier, SecondaryIdentifiers, FullName) = (ticks, identifier, secondaries, name);
    }
}
