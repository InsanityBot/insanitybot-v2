using System;

namespace InsanityBot.Utility.Exceptions
{
    public class ModlogNotFoundException : Exception
    {
        public UInt64 UserId { get; set; }

        public ModlogNotFoundException(String message, UInt64 id) : base(message) => this.UserId = id;
    }
}
