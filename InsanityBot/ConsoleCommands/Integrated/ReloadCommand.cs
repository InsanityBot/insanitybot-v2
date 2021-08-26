using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.ConsoleCommands.Integrated
{
    internal class ReloadCommand
    {
        public void ReloadConsoleCommand()
        {
            InsanityBot.UnloadAll();
            _ = InsanityBot.Main(Array.Empty<String>());
            GC.Collect();
        }
    }
}
