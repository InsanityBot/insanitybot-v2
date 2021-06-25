using DSharpPlus.CommandsNext;

using System;
using System.Collections.Generic;
using System.Linq;

namespace InsanityBot.Tickets.CustomCommands
{
    public sealed class CustomCommandHandler
    {
        public static readonly Dictionary<InternalCommand, Action<CommandContext, Object>> internalCommandMap = new()
        {
        };

        public List<Command> Commands { get; private set; }

        public void HandleCommand(CommandContext context, String command)
        {
            IEnumerable<Command> commands = from i in this.Commands
                                            where i.Trigger == command
                                            select i;
        }
    }
}
