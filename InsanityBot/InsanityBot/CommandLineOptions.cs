using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

namespace InsanityBot
{
    public class CommandLineOptions
    {
        [Option('i', "init", Required = false, Default = false, 
            HelpText = "Initialize all missing files on startup. This excludes modlog and permission files.")]
        public Boolean Initialize { get; set; }

        [Option('h', "hard-reset", Required = false, Default = false, HelpText = "Regenerate all files on startup, including the main config. " +
            "This will wipe your existing configuration.")]
        public Boolean HardReset { get; set; }

        [Option('d', "datafix", Required = false, Default = false, Hidden = true, HelpText = "Currently not in use.")]
        public Boolean ApplyDatafixes { get; set; }
    }
}
