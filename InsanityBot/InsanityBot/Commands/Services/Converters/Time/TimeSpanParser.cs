using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Emzi0767;

using static InsanityBot.Commands.Services.Converters.Time.DefaultTimeIdentifiers;

namespace InsanityBot.Commands.Services.Converters.Time
{
    public class TimeSpanParser
    {
        public TimeSpan ConvertToTimeSpan(String value)
        {
            if (TimeSpan.TryParse(value, out var tryParseResult))
                return tryParseResult;

            else
                return ParseTimeSpan(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private TimeSpan ParseTimeSpan(String value) //gonna have to comment this method :c
        {
            if (RemappedTimeIdentifiers == null)
                RemapIdentifiers();

            //will be parsed
            Dictionary<Char, Int32> splitValues = new Dictionary<Char, Int32>();

            //various things that need to survive one foreach loop
            Boolean IntegerSection = true;
            Byte[] Indices = new Byte[12];
            Char[] ValueArray = value.ToCharArray();
            Byte IndexPosition = 1;

            //this one is always going to be 0
            Indices[0] = 0;

            //now we figure out where to split
            for(Byte b = 0; b < ValueArray.Length; b++)
            {
                if (ValueArray[b].IsBasicDigit())
                {
                    //we are parsing an integer anyways, dont worry about it
                    if (IntegerSection)
                        continue;

                    //we met a new integer, worry about it
                    else
                    {
                        IntegerSection = true;
                        Indices[IndexPosition] = b;
                        IndexPosition++;
                    }
                }
                else if (ValueArray[b].IsBasicLetter() || ValueArray[b] == ' ')
                {
                    //we are parsing a letter anyways, dont worry about it
                    if (!IntegerSection)
                        continue;

                    //we met a new letter, panic!
                    else
                    {
                        IntegerSection = false;
                        Indices[IndexPosition] = b;
                        IndexPosition++;
                    }
                }
                else
                    throw new ArgumentException("Cannot parse non-alphanumeric characters");
            }

            /* Now we know where we want to split our string
             * this shouldnt be a problem, right?
             * 
             * 1. we know integer and string sections alternate
             * 2. we know our string is valid
             * 3. we know the positions we need to split
             * 
             * Thankfully, this code is not all that frequented so we do not have to worry about nanosecond performance.
             * Yet we still want it to run decently fast...
             * anyways, time to waste 512 bytes of ram... 
             * they technically arent wasted, in fact this is very efficient code, but it could be dynamically allocated
             * that would cost more CPU power but much less memory. since most of this code is CPU heavy, we should rather
             * focus on CPU performance though. Additionally, the JIT can still decide to allocate dynamically. */

            // create our temporary array
            String[] temporarilySplitParseable = new String[12];

            //think logically and add  the end of our string as the last possible value
            Indices[11] = (Byte)value.Length;

            //some more for looping just to split strings
            //flashbacks to parsing in C
            for (Byte b = 0; b < 11; b++)
                temporarilySplitParseable[b] = value[Indices[b]..Indices[b + 1]];

            /* now we parse our identifier aliases as defined in DefaultTimeIdentifiers
             * since we know the first one has to be an integer, we start with the 2nd element 
             * and always step by two elements */

            for(Byte b = 1; b < 12; b += 2)
            {
                if (RemappedTimeIdentifiers[Second.PrimaryIdentifier].Contains(temporarilySplitParseable[b]))
                    splitValues.Add(Second.PrimaryIdentifier, Convert.ToInt32(temporarilySplitParseable[b - 1]));
                if (RemappedTimeIdentifiers[Minute.PrimaryIdentifier].Contains(temporarilySplitParseable[b]))
                    splitValues.Add(Minute.PrimaryIdentifier, Convert.ToInt32(temporarilySplitParseable[b - 1]));
                if (RemappedTimeIdentifiers[Hour.PrimaryIdentifier].Contains(temporarilySplitParseable[b]))
                    splitValues.Add(Hour.PrimaryIdentifier, Convert.ToInt32(temporarilySplitParseable[b - 1]));
                if (RemappedTimeIdentifiers[Day.PrimaryIdentifier].Contains(temporarilySplitParseable[b]))
                    splitValues.Add(Day.PrimaryIdentifier, Convert.ToInt32(temporarilySplitParseable[b - 1]));
                if (RemappedTimeIdentifiers[Month.PrimaryIdentifier].Contains(temporarilySplitParseable[b]))
                    splitValues.Add(Month.PrimaryIdentifier, Convert.ToInt32(temporarilySplitParseable[b - 1]));
                if (RemappedTimeIdentifiers[Year.PrimaryIdentifier].Contains(temporarilySplitParseable[b]))
                    splitValues.Add(Year.PrimaryIdentifier, Convert.ToInt32(temporarilySplitParseable[b - 1]));
            }

            //the remapping is now completed, time to - finally - parse and return

            Int64 milliseconds = 0;

            foreach(var v in splitValues)
            {
                if (v.Key == Second.PrimaryIdentifier)
                    milliseconds += v.Value * Second.Ticks;
                if (v.Key == Minute.PrimaryIdentifier)
                    milliseconds += v.Value * Minute.Ticks;
                if (v.Key == Hour.PrimaryIdentifier)
                    milliseconds += v.Value * Hour.Ticks;
                if (v.Key == Day.PrimaryIdentifier)
                    milliseconds += v.Value * Day.Ticks;
                if (v.Key == Month.PrimaryIdentifier)
                    milliseconds += v.Value * Month.Ticks;
                if (v.Key == Year.PrimaryIdentifier)
                    milliseconds += v.Value * Year.Ticks;
            }

            return new TimeSpan(milliseconds);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RemapIdentifiers()
        {
            RemappedTimeIdentifiers = new Dictionary<Char, String[]>
            {
                { Second.PrimaryIdentifier, Second.SecondaryIdentifiers.Append(Second.FullName).ToArray() },
                { Minute.PrimaryIdentifier, Minute.SecondaryIdentifiers.Append(Minute.FullName).ToArray() },
                { Hour.PrimaryIdentifier, Hour.SecondaryIdentifiers.Append(Hour.FullName).ToArray() },
                { Day.PrimaryIdentifier, Day.SecondaryIdentifiers.Append(Day.FullName).ToArray() },
                { Month.PrimaryIdentifier, Month.SecondaryIdentifiers.Append(Month.FullName).ToArray() },
                { Year.PrimaryIdentifier, Year.SecondaryIdentifiers.Append(Year.FullName).ToArray() }
            };
        }

        private static Dictionary<Char, String[]> RemappedTimeIdentifiers { get; set; }
    }
}
