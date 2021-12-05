namespace InsanityBot.MessageServices.Messages.Rules;
using System;

using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;

using InsanityBot.MessageServices.Messages.Util;

public class FakeContextBuilder
{
	public LogContext BuildContext<T>(T eventArgs)
	{
		if(eventArgs is MessageDeleteEventArgs a)
		{
			return new(a.Message.Author, a.Channel, null, a.Message);
		}
		if(eventArgs is MessageUpdateEventArgs b)
		{
			return new(b.Message.Author, b.Channel, null, b.Message);
		}
		if(eventArgs is MessageBulkDeleteEventArgs c)
		{
			return new(null, c.Channel, null, null);
		}
		if(eventArgs is GuildMemberAddEventArgs d)
		{
			return new(d.Member, null, null, null);
		}
		if(eventArgs is GuildMemberRemoveEventArgs e)
		{
			return new(e.Member, null, null, null);
		}
		if(eventArgs is CommandExecutionEventArgs f)
		{
			return new(f.Context.Member, f.Context.Channel, f.Context.Command, f.Context.Message);
		}

		throw new ArgumentException("The given argument was no valid event argument type.");
	}
}