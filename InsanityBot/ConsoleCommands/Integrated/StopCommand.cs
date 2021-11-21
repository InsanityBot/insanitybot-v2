namespace InsanityBot.ConsoleCommands.Integrated;
using Microsoft.Extensions.Logging;

internal class StopCommand
{
	public void StopConsoleCommand()
	{
		InsanityBot.Client.Logger.LogInformation(new EventId(1100, "StopCommand"), $"Shutting down InsanityBot v{InsanityBot.Version}...");

		InsanityBot.Shutdown();
	}
}