using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static async Task Main(String[] args)
        {
            //load command line arguments
            CommandLine.InitializeCommandLine();
            CommandLine.InsanityBotApplication.Execute(args);

            //reset if reset flag is set
            if (InsanityBot.CommandLine.HardResetOnStartup.HasValue())
                await HardReset();

            //initialize if init flag is set
            if (InsanityBot.CommandLine.InitializeOnStartup.HasValue())
                await Initialize();

            //initialization finished, abort main thread
            await Task.Delay(-1);
        }
    }
}
