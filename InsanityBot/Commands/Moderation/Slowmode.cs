using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;
using Microsoft.Extensions.Logging;
using InsanityBot.Utility.Permissions;

namespace InsanityBot.Commands.Moderation
{
    public class Slowmode : BaseCommandModule
    {
        [Command("slowmode")]
        public async Task SlowmodeCommand(CommandContext ctx,
            DiscordChannel channel,

            [RemainingText]
            String args = "usedefault")
        {
            if(args.StartsWith('-'))
            {
                await ParseSlowmodeCommand(ctx, channel, args);
                return;
            }

            if(Int32.TryParse(args, out var t))
            {
                await ExecuteSlowmodeCommand(ctx, channel, t, false);
            }
            else
            {
                await ExecuteSlowmodeCommand(ctx, channel, ToInt32(InsanityBot.Config["insanitybot.commands.slowmode.default_slowmode"]), false);
            }
        }

        [Command("slowmode")]
        public async Task SlowmodeCommand(CommandContext ctx,
            [RemainingText]
            String args = "usedefault")
        {
            if(args.StartsWith('-'))
            {
                await ParseSlowmodeCommand(ctx, args);
                return;
            }

            if(Int32.TryParse(args, out var t))
            {
                await ExecuteSlowmodeCommand(ctx, ctx.Channel, t, false);
            }
            else
            {
                await ExecuteSlowmodeCommand(ctx, ctx.Channel, ToInt32(InsanityBot.Config["insanitybot.commands.slowmode.default_slowmode"]), false);
            }
        }

        private async Task ParseSlowmodeCommand(CommandContext ctx, DiscordChannel channel, String args)
        {
            try
            {
                await Parser.Default.ParseArguments<SlowmodeOptions>(args.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteSlowmodeCommand(ctx, channel, o.SlowmodeTime, o.Silent);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.failure"], ctx),
                    Color = DiscordColor.Red,
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel.SendMessageAsync(failed.Build());
            }
        }

        private async Task ParseSlowmodeCommand(CommandContext ctx, String args)
        {
            try
            {
                await Parser.Default.ParseArguments<SlowmodeOptions>(args.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        if (o.DiscordChannelId == 0)
                            await ExecuteSlowmodeCommand(ctx, ctx.Channel, o.SlowmodeTime, o.Silent);
                        else
                            await ExecuteSlowmodeCommand(ctx, InsanityBot.HomeGuild.GetChannel(o.DiscordChannelId), o.SlowmodeTime, o.Silent);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.failure"], ctx),
                    Color = DiscordColor.Red,
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel.SendMessageAsync(failed.Build());
            }
        }

        private async Task ExecuteSlowmodeCommand(CommandContext ctx, DiscordChannel channel, Int32 slowmodeTime, Boolean silent)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.slowmode"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            if (silent)
                await ctx.Message.DeleteAsync();

            DiscordEmbedBuilder embedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = new()
            {
                Title = "Slowmode",
                Color = DiscordColor.Blue,
                Footer = new()
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Channel", channel.Mention, true)
                .AddField("Time", slowmodeTime.ToString(), true);

            try
            {
                await channel.ModifyAsync(xm =>
                {
                    xm.PerUserRateLimit = slowmodeTime;
                });

                embedBuilder = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.success"],
                        ctx).Replace("{TIME}", slowmodeTime.ToString()),
                    Color = DiscordColor.Blue,
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch(Exception e)
            {
                embedBuilder = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.slowmode.failure"], ctx),
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
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
            }
        }
    }

    public class SlowmodeOptions : ModerationOptionBase
    {
        [Option('t', "slowmode-time", Default = 0, Required = false)]
        public Int32 SlowmodeTime { get; set; }

        [Option('c', "channel", Default = 0, Required = false)]
        public UInt64 DiscordChannelId { get; set; }
    }
}
