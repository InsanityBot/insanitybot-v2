using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Commands.Miscellaneous
{
    public class Embed : BaseCommandModule
    {
        [Command("embed")]
        public async Task SayEmbedCommand(CommandContext ctx,
            [RemainingText]
            String text)
        {
            if (!ctx.Member.HasPermission("insanitybot.miscellaneous.say.embed"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            _ = ctx.Message.DeleteAsync();
            DiscordEmbedBuilder embedBuilder = new()
            {
                Description = text,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                },
                Color = DiscordColor.Blurple
            };
            _ = ctx.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}
