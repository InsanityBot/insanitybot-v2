using DSharpPlus.CommandsNext;

using InsanityBot.Tickets.CustomCommands.Internal;

using System;
using System.Collections.Generic;
using System.Linq;

namespace InsanityBot.Tickets.CustomCommands
{
    public sealed class CustomCommandHandler
    {
        public List<Command> Commands { get; private set; }

        public void HandleCommand(CommandContext context, String command)
        {
            IEnumerable<Command> commands = from i in this.Commands
                                            where i.Trigger == command
                                            select i;

            foreach(var v in commands)
            {
                switch(v.InternalCommand)
                {
                    case InternalCommand.Close:
                        new CloseCommand().ProcessCloseCommand(context, v.Parameter);
                        break;
                    case InternalCommand.Move:
                        new MoveCommand().ProcessMoveCommand(context, v.Parameter);
                        break;
                    case InternalCommand.SendEmbed:
                        new SendEmbedCommand().ProcessSendCommand(context, v.Parameter);
                        break;
                    case InternalCommand.SendMessage:
                        new SendMessageCommand().ProcessSendCommand(context, v.Parameter);
                        break;
                    default:
                        throw new ArgumentException($"Invalid internal command type specified by command {v.Trigger}, " +
                            $"aborting command execution");
                }
            }
        }
    }
}
