using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

namespace InsanityBot.Commands.Miscellaneous
{
    public class Say : BaseCommandModule
    {
        [Command("say")]
        public async Task SayCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {
            if (!ctx.Member.HasPermission("insanitybot.miscellaneous.say"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            _ = ctx.Message.DeleteAsync();
            _ = ctx.RespondAsync(text);
        }

        [Command("embed")]
        public async Task SayEmbedCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {
            if(!ctx.Member.HasPermission("insanitybot.miscellaneous.say.embed"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            _ = ctx.Message.DeleteAsync();
            DiscordEmbedBuilder embedBuilder = new()
            {
                Description = text,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "https://github.com/InsanityNetwork/InsanityBot"
                },
                Color = DiscordColor.Blurple
            };
            _ = ctx.RespondAsync(embed: embedBuilder.Build());
        }
    }
}
