using DSharpPlus.CommandsNext;

using InsanityBot.Tickets.CustomCommands.Internal;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
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

        public void Load()
        {
            if(!File.Exists("./cache/tickets/customcommands.json"))
                File.Create("./cache/tickets/customcommands.json").Close();

            StreamReader reader = new("./cache/tickets/customcommands.json");

            Commands = JsonConvert.DeserializeObject<List<Command>>(reader.ReadToEnd());
            reader.Close();
        }

        public void Save()
        {
            // no init code, to be saved it has to be loaded first
            StreamWriter writer = new("./cache/tickets/customcommands.json");
            writer.Write(JsonConvert.SerializeObject(Commands));
            writer.Close();
        }
    }
}
