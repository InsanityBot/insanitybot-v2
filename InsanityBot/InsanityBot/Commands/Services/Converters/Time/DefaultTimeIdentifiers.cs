using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Commands.Services.Converters.Time
{
    internal struct DefaultTimeIdentifiers
    {
        internal static TimeIdentifier Second = new TimeIdentifier(1000, 's', "second", "sec", "seconds");
        internal static TimeIdentifier Minute = new TimeIdentifier(60000, 'm', "minute", "min", "minutes");
        internal static TimeIdentifier Hour = new TimeIdentifier(3600000, 'h', "hour", "hr", "hrs", "hours");
        internal static TimeIdentifier Day = new TimeIdentifier(86400000, 'd', "day", "d", "days");
        internal static TimeIdentifier Week = new TimeIdentifier(604800000, 'w', "week", "weeks");
        internal static TimeIdentifier Month = new TimeIdentifier(2630016000, 'o', "month", "months", "mo");
        internal static TimeIdentifier Year = new TimeIdentifier(31536000000, 'y', "year", "years", "yr", "yrs");
    }
}
