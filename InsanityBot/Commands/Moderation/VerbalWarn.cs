using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs; // unsafe interface to allow faster method chaining
using InsanityBot.Utility.Modlogs.SafeAccessInterface;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static System.Convert;
using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public class VerbalWarn : BaseCommandModule
    {
        [Command("verbalwarn")]
        [Aliases("vw", "verbal-warn")]
        public async Task VerbalWarnCommand(CommandContext ctx,
            DiscordMember member,

            [RemainingText]
            String arguments = "usedefault")
        {
            if (!(Boolean)InsanityBot.Config["insanitybot.commands.moderation.allow_minor_warns"])
            {
                return;
            }

            if (arguments.StartsWith('-'))
            {
                await ParseVerbalWarnCommand(ctx, member, arguments);
                return;
            }
            await ExecuteVerbalWarnCommand(ctx, member, arguments, false, false);
        }

        private async Task ParseVerbalWarnCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                {
                    cmdArguments += " --reason usedefault";
                }

                await Parser.Default.ParseArguments<VerbalWarnOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteVerbalWarnCommand(ctx, member, String.Join(' ', o.Reason), o.Silent, o.DmMember);
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.verbal_warn.failure"],
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

        private async Task ExecuteVerbalWarnCommand(CommandContext ctx,
            DiscordMember member,
            String reason,
            Boolean silent,
            Boolean dmMember)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.verbal_warn"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            if (silent)
            {
                await ctx.Message.DeleteAsync();
            }

            String VerbalWarnReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                    ctx, member),
                _ => GetFormattedString(reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null, moderationEmbedBuilder = new()
            {
                Title = "Verbal Warn",
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", VerbalWarnReason, true);

            try
            {
                _ = member.TryAddVerballogEntry(VerbalWarnReason);

                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetMemberReason(InsanityBot.LanguageConfig["insanitybot.moderation.verbal_warn.reason"],
                        VerbalWarnReason, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.verbal_warn.failure"],
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
                await ctx.Channel.SendMessageAsync(embedBuilder.Build());

                if ((Boolean)InsanityBot.Config["insanitybot.commands.moderation.convert_minor_warns_into_full_warn"])
                {
                    if ((member.GetUserModlog().VerbalLogEntryCount %
                        (Int64)InsanityBot.Config["insanitybot.commands.moderation.minor_warns_equal_full_warn"]) == 0)
                    {
                        await new Warn().WarnCommand(ctx, member, $"--silent --reason Too many verbal warns, count since last warn exceeded " +
                            $"{(Int64)InsanityBot.Config["insanitybot.commands.moderation.minor_warns_equal_full_warn"]}");
                    }
                }
            }
        }
    }

    public class VerbalWarnOptions : ModerationOptionBase
    {

    }
}
