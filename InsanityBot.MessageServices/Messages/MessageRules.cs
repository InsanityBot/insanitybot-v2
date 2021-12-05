namespace InsanityBot.MessageServices.Messages;
using System;
using System.Collections.Generic;

using DSharpPlus.Entities;

using InsanityBot.MessageServices.Messages.Rules;

public class MessageRules
{
	public Dictionary<LogEvent, DiscordChannel> Defaults { get; set; }
	public Dictionary<LogEvent, LoggerRule[]> Rules { get; set; }
	public Dictionary<UInt64, ILoggingGateway> Channels { get; set; }

	private readonly DiscordGuild _guild;

	public MessageRules(DiscordGuild guild)
	{
		this._guild = guild;
		this.Defaults = new();
		this.Rules = new();
		this.Channels = new();
	}
}