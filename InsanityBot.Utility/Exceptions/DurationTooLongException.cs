using System;

namespace InsanityBot.Utility.Exceptions
{
    public class DurationTooLongException : Exception
    {
        public DurationTooLongException(String message) : base(message)
        {
        }
    }
}
