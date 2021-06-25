using DSharpPlus.CommandsNext;

using System;
using System.Threading;

using TimeSpanParserUtil;

namespace InsanityBot.Tickets.CustomCommands.Internal
{
    internal class CloseCommand
    {
        public void ProcessCloseCommand(CommandContext context, Object parameter)
        {
            TimeSpan time;

            if(parameter is TimeSpan)
            {
                time = (TimeSpan)parameter;
            }
            else if(parameter is Int64)
            {
                time = new TimeSpan((Int64)parameter);
            }
            else if(parameter is String)
            {
                time = TimeSpanParser.Parse((String)parameter);
            }
            else
            {
                throw new ArgumentException("Invalid datatype for close command delay.", nameof(parameter));
            }

            Thread.Sleep(time);

            context.Channel.DeleteAsync();
        }
    }
}
