using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;

using System;

namespace InsanityBot.MessageServices.Messages.Rules
{
    public class FakeContextBuilder
    {
        private CommandsNextExtension CommandsExtension { get; set; }

        public FakeContextBuilder(CommandsNextExtension extension)
        {
            this.CommandsExtension = extension;
        }

        public CommandContext BuildContext<T>(T eventArgs)
        {
            if(eventArgs is MessageDeleteEventArgs a)
            {
                return CommandsExtension.CreateFakeContext(a.Message.Author, a.Channel, a.Message.Content, null, null);
            }
            if(eventArgs is MessageUpdateEventArgs b)
            {
                return CommandsExtension.CreateFakeContext(b.Message.Author, b.Channel, b.Message.Content, 
                    null, null, b.MessageBefore.Content);
            }
            if(eventArgs is MessageBulkDeleteEventArgs c)
            {
                return CommandsExtension.CreateFakeContext(null, c.Channel, null, null, null, c.Messages.Count.ToString());
            }
            if(eventArgs is GuildMemberAddEventArgs d)
            {
                return CommandsExtension.CreateFakeContext(d.Member, null, null, null, null);
            }
            if(eventArgs is GuildMemberRemoveEventArgs e)
            {
                return CommandsExtension.CreateFakeContext(e.Member, null, null, null, null);
            }
            if(eventArgs is CommandExecutionEventArgs f)
            {
                return f.Context;
            }

            throw new ArgumentException("The given argument was no valid event argument type.");
        }
    }
}
