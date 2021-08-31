using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Attributes;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    [Group("slowmode")]
    public class Slowmode : BaseCommandModule
    {
        [GroupCommand]
        [RequirePermission("insanitybot.moderation.slowmode")]
        public async Task SlowmodeCommand(CommandContext ctx,
            DiscordChannel channel,

            [RemainingText]
            String args = "usedefault")
        {
            if(args.StartsWith('-'))
            {
                await this.ParseSlowmodeCommand(ctx, channel, args);
                return;
            }

            try
            {
                await this.ExecuteSlowmodeCommand(ctx, channel, args.ParseTimeSpan(), false);
            }
            catch
            {
                await this.ExecuteSlowmodeCommand(ctx, channel, InsanityBot.Config.Value<String>(
                    "insanitybot.commands.slowmode.default_slowmode")
                        .ParseTimeSpan(), false);
            }
        }

        [GroupCommand]
        public async Task SlowmodeCommand(CommandContext ctx,
            [RemainingText]
            String args = "usedefault")
            => await this.SlowmodeCommand(ctx, ctx.Channel, args);

        private async Task ParseSlowmodeCommand(CommandContext ctx, DiscordChannel channel, String args)
        {
            try
            {
                await Parser.Default.ParseArguments<SlowmodeOptions>(args.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await this.ExecuteSlowmodeCommand(ctx, channel, o.SlowmodeTime.ParseTimeSpan(), o.Silent);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.failure"], ctx));

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel?.SendMessageAsync(failed.Build());
            }
        }

        private async Task ExecuteSlowmodeCommand(CommandContext ctx, DiscordChannel channel, TimeSpan slowmodeTime, Boolean silent, Boolean auto = false)
        {
            if(silent)
            {
                await ctx.Message?.DeleteAsync();
            }

            DiscordEmbedBuilder embedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.slowmode"];

            moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
                .AddField("Channel", channel.Mention, true)
                .AddField("Time", slowmodeTime.ToString(), true);

            try
            {
                await channel.ModifyAsync(xm =>
                {
                    xm.PerUserRateLimit = slowmodeTime.Seconds;
                });

                embedBuilder = InsanityBot.Embeds["insanitybot.moderation.slowmode"]
                    .WithDescription(auto switch
                    {
                        false => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.success"], ctx)
                            .Replace("{TIME}", slowmodeTime.ToString()),
                        true => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.reset.success"], ctx)
                    });

                _ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder
                }, ctx);
            }
            catch(Exception e)
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(auto switch
                    {
                        false => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.failure"], ctx),
                        true => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.reset.failure"], ctx)
                    });

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                if(!silent)
                {
                    await ctx.Channel?.SendMessageAsync(embedBuilder.Build());
                }
            }
        }

        [Command("reset")]
        [Aliases("remove", "clear")]
        [RequirePermission("insanitybot.moderation.slowmode")]
        public async Task ResetSlowmodeCommand(CommandContext ctx, Boolean silent = false)
            => await this.ResetSlowmodeCommand(ctx, ctx.Channel, silent);

        [Command("reset")]
        public async Task ResetSlowmodeCommand(CommandContext ctx,
            DiscordChannel channel, Boolean silent = false)
            => await this.ExecuteSlowmodeCommand(ctx, channel, new(0), silent, true);
    }

    public class SlowmodeOptions : ModerationOptionBase
    {
        [Option('t', "slowmode-time", Default = "0s", Required = false)]
        public String SlowmodeTime { get; set; }

        [Option('c', "channel", Default = 0, Required = false)]
        public UInt64 DiscordChannelId { get; set; }
    }
}
