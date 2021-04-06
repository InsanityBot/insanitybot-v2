using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity.Enums;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public partial class Modlog : BaseCommandModule
    {
        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx,
            DiscordMember user)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.modlog"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            try
            {
                UserModlog modlog = user.GetUserModlog();

                DiscordEmbedBuilder modlogEmbed = new()
                {
                    Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                        ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                if (modlog.ModlogEntryCount == 0)
                {
                    modlogEmbed.Color = DiscordColor.SpringGreen;
                    modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.empty_modlog"],
                        ctx, user);
                    _ = ctx.RespondAsync(embed: modlogEmbed.Build());
                }
                else
                {
                    if (!ToBoolean(InsanityBot.Config["insanitybot.commands.modlog.allow_scrolling"]))
                    {
                        modlogEmbed.Color = DiscordColor.Red;
                        modlogEmbed.Description = user.CreateModlogDescription(false);

                        await ctx.RespondAsync(embed: modlogEmbed.Build());
                    }
                    else
                    {
                        modlogEmbed.Color = DiscordColor.Red;
                        String embedDescription = user.CreateModlogDescription();

                        var interactivity = ctx.Client.GetInteractivity();

                        var pages = interactivity.GeneratePagesInEmbed(embedDescription, SplitType.Line, modlogEmbed);

                        await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
                    }
                }
            }
            catch (Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1170, "Modlog"), $"Could not retrieve modlogs: {e}: {e.Message}");

                DiscordEmbedBuilder failedModlog = new()
                {
                    Color = DiscordColor.Red,
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.failed"], ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                await ctx.RespondAsync(embed: failedModlog.Build());
            }
        }

        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx)
            => await ModlogCommand(ctx, ctx.Member);
    }
}
