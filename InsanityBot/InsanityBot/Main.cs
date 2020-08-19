using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static async Task Main(string[] args)
        {
            

            //load command line arguments
            CommandLine.InitializeCommandLine();
            CommandLine.InsanityBotApplication.Execute(args);

            //initialization finished, abort main thread
            await Task.Delay(-1);
        }
    }
}
