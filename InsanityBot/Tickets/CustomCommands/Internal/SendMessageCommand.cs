using DSharpPlus.CommandsNext;

using System;

namespace InsanityBot.Tickets.CustomCommands.Internal
{
    public class SendMessageCommand
    {
        public void ProcessSendCommand(CommandContext context, Object parameter)
        {
            if(parameter is not String value)
            {
                throw new ArgumentException("Invalid datatype for message string.", nameof(parameter));
            }

            context.Channel?.SendMessageAsync(value);
        }
    }
}
