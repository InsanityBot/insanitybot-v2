namespace InsanityBot;
using System;

using CommandLine;

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

	[Option("interactive", Required = false, Default = true, HelpText = "Allows to start up the bot with default configs, you will then be asked to enter critical values in console")]
	public Boolean Interactive { get; set; }
}