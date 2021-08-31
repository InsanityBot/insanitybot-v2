using System;

namespace InsanityBot.ConsoleCommands.Integrated
{
    internal class ReloadCommand
    {
        public void ReloadConsoleCommand()
        {
            Console.WriteLine("This will fully reload the entire bot. Do you really wish to proceed?");

            if(Console.ReadLine().ToLower() == "n")
            {
                return;
            }

            InsanityBot.UnloadAll();
            _ = InsanityBot.Main(Array.Empty<String>());
            GC.Collect();
        }
    }
}
