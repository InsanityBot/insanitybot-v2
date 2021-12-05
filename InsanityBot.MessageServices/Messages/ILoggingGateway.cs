namespace InsanityBot.MessageServices.Messages;
using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;

public interface ILoggingGateway
{
	public Task SendMessage(DiscordEmbed embed);
	public Task SendMessage(String content);
	public Task SendMessage(DiscordMessageBuilder builder);

	public static ILoggingGateway Empty
	{
		get;
	}
}