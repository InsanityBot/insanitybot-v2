using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace InsanityBot.Console.Integrated
{
    public static class IntegratedCommandHandler
    {
        public static void Initialize()
        {
            _ = Task.Run(() => { AwaitConsoleCommand(); });
            InsanityBot.Client.Logger.LogInformation(new EventId(1001, "ConsoleMain"), "Loading InsanityBot integrated console commands successful");
        }

        private static void AwaitConsoleCommand()
        {
            String command;

            do
            {
                command = System.Console.ReadLine();

                switch(command)
                {
                    case "stop":
                    case "exit":
                    case "return":
                        new StopCommand().StopConsoleCommand();
                        break;
                    default:
                        break;
                }
            }
            while(command != "stop" && command != "exit" && command != "return");
        }
    }
}
