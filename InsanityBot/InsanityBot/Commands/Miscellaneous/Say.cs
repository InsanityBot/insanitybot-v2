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
            if (!(await InsanityBot.PermissionManager.GetCacheEntry(ctx.Member.Id))["insanitybot.miscellaneous.say"])
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            _ = ctx.Message.DeleteAsync();
            _ = ctx.RespondAsync(text);
        }

        [Command("sayembed")]
        public async Task SayEmbedCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {
            if(!(await InsanityBot.PermissionManager.GetCacheEntry(ctx.Member.Id))["insanitybot.miscellaneous.say.embed"])
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }
        }
    }
}
