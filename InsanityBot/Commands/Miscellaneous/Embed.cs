using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using InsanityBot.Core.Attributes;
using InsanityBot.Core.Formatters.Embeds;

namespace InsanityBot.Commands.Miscellaneous
{
    public class Embed : BaseCommandModule
    {
        [Command("embed")]
        [RequirePermission("insanitybot.miscellaneous.say.embed")]
        public async Task SayEmbedCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {
            await ctx.Message?.DeleteAsync();
            _ = ctx.Channel?.SendMessageAsync((InsanityBot.EmbedFactory.GetFormatter() as EmbedFormatter).Read(text));
        }
    }
}
