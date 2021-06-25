using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using System;

namespace InsanityBot.Tickets.CustomCommands.Internal
{
    internal class MoveCommand
    {
        public void ProcessMoveCommand(CommandContext context, Object parameter)
        {
            if(parameter is not UInt64 destination)
            {
                throw new ArgumentException("Invalid datatype for move command destination.", nameof(parameter));
            }

            DiscordChannel category = InsanityBot.HomeGuild.GetChannel(destination);

            if(!category.IsCategory)
            {
                throw new ArgumentException("Cannot move a channel to another channel.", nameof(parameter));
            }

            context.Channel.ModifyAsync(xm =>
            {
                xm.Parent = category;
            });
        }
    }
}
