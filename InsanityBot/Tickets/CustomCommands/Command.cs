using System;

namespace InsanityBot.Tickets.CustomCommands
{
    public record Command
    {
        public String Trigger { get; init; }
        public InternalCommand InternalCommand { get; init; }
    }
}
