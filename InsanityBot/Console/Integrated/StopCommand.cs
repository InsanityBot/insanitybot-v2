using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace InsanityBot.Console.Integrated
{
    internal class StopCommand
    {
        public void StopConsoleCommand()
        {
            InsanityBot.Client.Logger.LogInformation(new EventId(1100, "StopCommand"), $"Shutting down InsanityBot v{InsanityBot.Version}...");
            Environment.Exit(0);
        }
    }
}
