using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation.Locking
{
    public class Unlock : BaseCommandModule
    {
        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx)
        {
            await UnlockCommand(ctx, ctx.Channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);
        }

        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx, DiscordChannel channel)
        {
            await UnlockCommand(ctx, channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);
        }

        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx, String args)
        {
            try
            {
                String cmdArguments = args;

                if (!args.Contains("-r") && !args.Contains("--reason"))
                    cmdArguments += " --reason usedefault";

                await Parser.Default.ParseArguments<LockOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await UnlockCommand(ctx, InsanityBot.HomeGuild.GetChannel(o.ChannelId), String.Join(' ', o.Reason), o.Silent);
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unlock.failure"],
                        ctx),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.RespondAsync(embed: failed.Build());
            }
        }

        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx, DiscordChannel channel, String args)
        {
            await UnlockCommand(ctx, args + $" -c {channel.Id}");
        }

        private async Task UnlockCommand(CommandContext ctx, DiscordChannel channel, String reason = "usedefault", Boolean silent = false)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.unlock"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            String UnlockReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], ctx),
                _ => GetFormattedString(reason, ctx)
            };

            DiscordEmbedBuilder embedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = new()
            {
                Title = "UNLOCK",
                Color = DiscordColor.Blue,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Channel", channel.Mention, true)
                .AddField("Reason", UnlockReason, true);

            try
            {
                var overwrites = channel.GetChannelData();
                var cachedData = channel.GetCachedChannelData();

                UInt64 exemptRole;
                if ((exemptRole = Convert.ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.lock_exempt_role_id"])) != 0)
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(exemptRole), allow: Permissions.None, reason:
                        "InsanityBot - unlocking channel, removing whitelist");

                foreach (var v in cachedData.LockedRoles)
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), deny: Permissions.None, reason: "InsanityBot - unlocking channel, removing permission overwrites");

                foreach (var v in cachedData.LockedRoles)
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), allow: Permissions.None, reason: "InsanityBot - unlocking channel, removing permission overwrites");

                foreach (var v in overwrites)
                    await channel.AddOverwriteAsync(await v.GetRoleAsync(), v.Allowed, v.Denied, "InsanityBot - unlocking channel, restoring previous permissions");

                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unlock.success"], ctx),
                    Color = DiscordColor.Blue,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unlock.failure"], ctx),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                if (!silent)
                    await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }
    }
}
