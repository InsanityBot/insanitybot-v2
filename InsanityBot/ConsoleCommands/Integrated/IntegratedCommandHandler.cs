using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace InsanityBot.ConsoleCommands.Integrated
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
                command = Console.ReadLine();

                switch(command)
                {
                    case "stop":
                    case "exit":
                    case "return":
                        new StopCommand().StopConsoleCommand();
                        break;
                    case "reload":
                        new ReloadCommand().ReloadConsoleCommand();
                        break;
                    default:
                        break;
                }
            }
            while(command != "stop" && command != "exit" && command != "return");
        }
    }
}
