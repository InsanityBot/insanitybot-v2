using System;
using System.Collections.Generic;
using System.Text;

namespace InsanityBot.Utility.Config.Exceptions
{
    public class ApplicationOverrideException : Exception
    {
        public Uri File { get; set; }

        public ApplicationOverrideException(String message) : base(message)
        { }

        public ApplicationOverrideException(String message, Exception innerException) : base(message, innerException)
        { }

        public ApplicationOverrideException()
        {
            File = null;
        }
    }
}
