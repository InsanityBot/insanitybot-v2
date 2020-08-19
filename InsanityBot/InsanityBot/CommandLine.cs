using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.CommandLineUtils;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static class CommandLine
        {

            public static CommandLineApplication InsanityBotApplication;

            public static CommandOption InitializeOnStartup { get; private set; }
            public static CommandOption HardResetOnStartup { get; private set; }

            internal static void InitializeCommandLine()
            {

                InsanityBotApplication = new CommandLineApplication
                {
                    Name = "InsanityBot",
                    Description = "C# Discord bot by ExaInsanity#1870",
                    ExtendedHelpText = "A C#-based, fully configurable Discord Bot using .NET Core 3.1 and DSharpPlus 4.0.0. " +
                    "Used in the Insanity Network Discord server and its related servers."
                };

                //register the Help option
                InsanityBotApplication.HelpOption("-?|-h|--help");

                //create the Version option
                InsanityBotApplication.VersionOption("-v|--version", () =>
                {
                    return $"InsanityBot Version: {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";
                });

                //register reinitialization option
                InitializeOnStartup = InsanityBotApplication.Option("-i|--init",
                    "Causes the bot to create all necessary files that do not exist yet. Recommended to run after longer downtime to get back up to date " +
                    "with people joining/leaving the guild.",
                    CommandOptionType.NoValue);

                //register hard reset option
                HardResetOnStartup = InsanityBotApplication.Option("-r|--reset",
                    "Causes the bot to create all necessary files from scratch, including configuration, modlog, experience and all other file-driven data. " +
                    "Recommended to run once when setting up the bot or when repurposing the server.",
                    CommandOptionType.NoValue);
            }
        }
    }
}
