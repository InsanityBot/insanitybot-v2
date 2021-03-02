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

                DiscordEmbedBuilder modlogEmbed = new DiscordEmbedBuilder
                {
                    Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                        ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - Exa 2020-2021"
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
                        modlogEmbed.Description = user.GetModlogEntries().ConvertToString();

                        if (modlog.ModlogEntryCount > ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]))
                        {
                            modlogEmbed.Description += GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.overflow"],
                                ctx, user);
                        }

                        await ctx.RespondAsync(embed: modlogEmbed.Build());
                    }
                    else
                    {
                        modlogEmbed.Color = DiscordColor.Red;
                        modlogEmbed.Description = user.GetModlogEntries().ConvertToString();
                        if (ReactionForwards == null)
                        {
                            try
                            {
                                ModlogMessageTracker.CreateTracker();

                                ReactionForwards = await InsanityBot.HomeGuild.GetEmojiAsync(ToUInt64(
                                    InsanityBot.Config["insanitybot.identifiers.modlog.scroll_right_emote_id"]));
                                ReactionBackwards = await InsanityBot.HomeGuild.GetEmojiAsync(ToUInt64(
                                    InsanityBot.Config["insanitybot.identifiers.modlog.scroll_left_emote_id"]));
                            }
                            catch
                            {
                                ReactionForwards = DiscordEmoji.FromName(InsanityBot.Client, ":arrow_forward:");
                                ReactionBackwards = DiscordEmoji.FromName(InsanityBot.Client, ":arrow_backward:");
                            }
                        }

                        var message = await ctx.RespondAsync(embed: modlogEmbed.Build());
                        _ = message.CreateReactionAsync(ReactionBackwards);
                        await Task.Delay(251); // delay the next reaction by 251ms to avoid getting ratelimited
                        _ = message.CreateReactionAsync(ReactionForwards);

                        await Task.Delay(500); // delay registering by 500ms to avoid false positives on reactions

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            Page = 0,
                            UserId = user.Id,
                            Type = ModlogMessageTracker.LogType.Modlog
                        });
                    }
                }
            }
            catch (Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1170, "Modlog"), $"Could not retrieve modlogs: {e}: {e.Message}");

                DiscordEmbedBuilder failedModlog = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.failed"], ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
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
