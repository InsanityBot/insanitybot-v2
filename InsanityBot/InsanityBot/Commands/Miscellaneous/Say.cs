using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace InsanityBot.Commands.Miscellaneous
{
    public class Say : BaseCommandModule
    {
        [Command("say")]
        public async Task SayCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {
            _ = ctx.Message.DeleteAsync();
            await ctx.RespondAsync(text);
        }

        [Command("say embed")]
        public async Task SayEmbedCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {

        }
    }
}
