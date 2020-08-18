using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.CommandLineUtils;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static CommandLineApplication InsanityBotApplication;
        public static async Task Main(string[] args)
        {
            InitializeCommandLine();
        }

        private static void InitializeCommandLine()
        {

            InsanityBotApplication = new CommandLineApplication
            {
                Name = "InsanityBot",
                Description = "C# Discord bot by ExaInsanity#1870",
                ExtendedHelpText = "A C#-based, fully configurable Discord Bot using .NET Core 3.1 and DSharpPlus 4.0.0 " +
                "Used in the Insanity Network Discord server and its related servers."
            };

            //register the Help option
            InsanityBotApplication.HelpOption("-?|-h|--help");

            //create the Version option
            InsanityBotApplication.VersionOption("-v|--version", () =>
            {
                return $"InsanityBot Version: {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";
            });
        }
    }
}
