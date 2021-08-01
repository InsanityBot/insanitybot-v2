using Microsoft.Extensions.Logging;

using System;

namespace InsanityBot.ConsoleCommands.Integrated
{
    internal class StopCommand
    {
        public void StopConsoleCommand()
        {
            InsanityBot.Client.Logger.LogInformation(new EventId(1100, "StopCommand"), $"Shutting down InsanityBot v{InsanityBot.Version}...");

            InsanityBot.Shutdown();
        }
    }
}
