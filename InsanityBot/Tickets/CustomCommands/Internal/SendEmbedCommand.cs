namespace InsanityBot.Tickets.CustomCommands.Internal;
using System;

using DSharpPlus.CommandsNext;

using global::InsanityBot.Core.Formatters.Embeds;

public class SendEmbedCommand
{
	public void ProcessSendCommand(CommandContext context, Object parameter)
	{
		if(parameter is not String embed)
		{
			throw new ArgumentException("Invalid datatype for embed format string.", nameof(parameter));
		}

		context.Channel?.SendMessageAsync((InsanityBot.EmbedFactory.GetFormatter() as EmbedFormatter).Read(embed));
	}
}