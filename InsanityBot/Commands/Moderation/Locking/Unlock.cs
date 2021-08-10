using CommandLine;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation.Locking
{
    public class Unlock : BaseCommandModule
    {
        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx) => await this.UnlockCommand(ctx, ctx.Channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);

        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx, DiscordChannel channel) => await this.UnlockCommand(ctx, channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);

        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx, String args)
        {
            try
            {
                String cmdArguments = args;

                if(!args.Contains("-r") && !args.Contains("--reason"))
                {
                    cmdArguments += " --reason usedefault";
                }

                await Parser.Default.ParseArguments<LockOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await this.UnlockCommand(ctx, InsanityBot.HomeGuild.GetChannel(o.ChannelId), String.Join(' ', o.Reason), o.Silent);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unlock.failure"], ctx)); 

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel?.SendMessageAsync(embed: failed.Build());
            }
        }

        [Command("unlock")]
        public async Task UnlockCommand(CommandContext ctx, DiscordChannel channel, String args) => await this.UnlockCommand(ctx, args + $" -c {channel.Id}");

        private async Task UnlockCommand(CommandContext ctx, DiscordChannel channel, String reason = "usedefault", Boolean silent = false)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.unlock"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            String UnlockReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], ctx),
                _ => GetFormattedString(reason, ctx)
            };

            DiscordEmbedBuilder embedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.unlock"];

            moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
                .AddField("Channel", channel.Mention, true)
                .AddField("Reason", UnlockReason, true);

            try
            {
                List<DiscordOverwrite> overwrites = channel.GetChannelData();
                ChannelData cachedData = channel.GetCachedChannelData();

                UInt64 exemptRole;
                if((exemptRole = InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.lock_exempt_role_id")) != 0)
                {
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(exemptRole), allow: DSharpPlus.Permissions.None, reason:
                        "InsanityBot - unlocking channel, removing whitelist");
                }

                foreach(UInt64 v in cachedData.LockedRoles)
                {
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), deny: DSharpPlus.Permissions.None, reason: "InsanityBot - unlocking channel, removing permission overwrites");
                }

                foreach(UInt64 v in cachedData.LockedRoles)
                {
                    await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), allow: DSharpPlus.Permissions.None, reason: "InsanityBot - unlocking channel, removing permission overwrites");
                }

                foreach(DiscordOverwrite v in overwrites)
                {
                    await channel.AddOverwriteAsync(await v.GetRoleAsync(), v.Allowed, v.Denied, "InsanityBot - unlocking channel, restoring previous permissions");
                }

                embedBuilder = InsanityBot.Embeds["insanitybot.moderation.unlock"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unlock.success"], ctx));
            }
            catch(Exception e)
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unlock.failure"], ctx));

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                if(!silent)
                {
                    await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }
    }
}
