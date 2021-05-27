using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public class ClearModlog : BaseCommandModule
    {
        [Command("clearmodlog")]
        public async Task ClearModlogCommand(CommandContext ctx,
            DiscordMember member,
            String arguments = "usedefault")
        {

            if (arguments.StartsWith('-'))
            {
                await ParseClearModlogCommand(ctx, member, arguments);
                return;
            }
            await ExecuteClearModlogCommand(ctx, member, false, arguments);
        }

        private async Task ParseClearModlogCommand(CommandContext ctx,
            DiscordMember member,
            String arguments = "usedefault")
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                {
                    cmdArguments += " --reason usedefault";
                }

                await Parser.Default.ParseArguments<ClearModlogOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteClearModlogCommand(ctx, member, o.Silent, String.Join(' ', o.Reason));
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.clear_modlog.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel.SendMessageAsync(embed: failed.Build());
            }
        }

        private async Task ExecuteClearModlogCommand(CommandContext ctx,
            DiscordMember member,
            Boolean silent,
            String reason)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.clear_modlog"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            if (silent)
            {
                await ctx.Message.DeleteAsync();
            }

            String ClearReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                    ctx, member),
                _ => GetFormattedString(reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null, moderationEmbedBuilder = new()
            {
                Title = "Modlog Cleared",
                Color = DiscordColor.SpringGreen,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", ClearReason, true);

            try
            {
                File.Delete($"./data/{member.Id}/modlog.json");
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.clear_modlog.success"],
                        ctx, member),
                    Color = DiscordColor.SpringGreen,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Moderation, new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder.Build()
                });
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.clear_modlog.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                if (!silent)
                {
                    await ctx.Channel.SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }
    }

    public class ClearModlogOptions : ModerationOptionBase
    {

    }
}
