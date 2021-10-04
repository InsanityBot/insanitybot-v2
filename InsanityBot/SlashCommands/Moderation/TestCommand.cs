using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace InsanityBot.SlashCommands.Moderation
{
    public class TestCommand : ApplicationCommandModule
    {
        [SlashCommand("test", "this sucks")]
        public async Task Test(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new()
            {
                Content = "this sucks"
            });
        }
    }
}