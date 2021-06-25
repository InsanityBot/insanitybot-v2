using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Modlogs.SafeAccessInterface;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public partial class Warn : BaseCommandModule
    {
        [Command("warn")]
        public async Task WarnCommand(CommandContext ctx,
            DiscordMember target,

            [RemainingText]
            String arguments = "usedefault")
        {
            if(arguments.StartsWith('-'))
            {
                await this.ParseWarnCommand(ctx, target, arguments);
                return;
            }
            await this.ExecuteWarn(ctx, target, arguments, false, false);
        }

        private async Task ParseWarnCommand(CommandContext ctx, DiscordMember target, String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if(!arguments.Contains("-r") && !arguments.Contains("--reason"))
                {
                    cmdArguments += " --reason usedefault";
                }

                await Parser.Default.ParseArguments<WarnOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await this.ExecuteWarn(ctx, target, String.Join(' ', o.Reason), o.Silent, o.DmMember);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"],
                        ctx, target),
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

        private async Task ExecuteWarn(CommandContext ctx,
            DiscordMember target,
            String reason,
            Boolean silent,
            Boolean dmMember)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.warn"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //if silent - delete the warn command
            if(silent)
            {
                await ctx.Message.DeleteAsync();
            }

            //actually do something with the usedefault value
            String WarnReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, target),
                _ => GetFormattedString(reason, ctx, target)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new()
            {
                Title = "Warn",
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", target.Mention, true)
                .AddField("Reason", WarnReason, true);

            try
            {
                _ = target.TryAddModlogEntry(ModlogEntryType.warn, WarnReason);
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.success"],
                        ctx, target),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Moderation, new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder
                });
            }
            catch(Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"],
                        ctx, target),
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
                if(!silent)
                {
                    await ctx.Channel.SendMessageAsync(embed: embedBuilder.Build());
                }

                if(dmMember)
                {
                    embedBuilder.Description = GetReason(InsanityBot.LanguageConfig["insanitybot.moderation.warn.reason"], WarnReason);
                    await (await target.CreateDmChannelAsync()).SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }
    }


    public class WarnOptions : ModerationOptionBase
    {

    }
}
