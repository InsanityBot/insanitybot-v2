using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static System.Convert;
using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public class Whitelist : BaseCommandModule
    {
        [Command("whitelist")]
        public async Task WhitelistCommand(CommandContext ctx,
            DiscordMember member,

            [RemainingText]
            String Reason = "usedefault")
        {
            if (Reason.StartsWith('-'))
            {
                await ParseWhitelistCommand(ctx, member, Reason);
                return;
            }
            await ExecuteWhitelistCommand(ctx, member, Reason, false, false);
        }

        private async Task ParseWhitelistCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                    cmdArguments += " --reason usedefault";

                await Parser.Default.ParseArguments<WhitelistOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteWhitelistCommand(ctx, member, String.Join(' ', o.Reason), o.Silent, o.DmMember);
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.RespondAsync(embed: failed.Build());
            }
        }

        private async Task ExecuteWhitelistCommand(CommandContext ctx,
            DiscordMember member,
            String Reason,
            Boolean Silent,
            Boolean DmMember)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.whitelist"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //actually do something with the usedefault value
            String WhitelistReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new()
            {
                Title = "Whitelist",
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", WhitelistReason, true);

            try
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.success"],
                        ctx, member),
                    Color = DiscordColor.White,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                _ = member.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
                    ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.blacklist_role_id"])),
                    WhitelistReason);
                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.failure"],
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
                await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }
    }

    public class WhitelistOptions : ModerationOptionBase
    {

    }
}

