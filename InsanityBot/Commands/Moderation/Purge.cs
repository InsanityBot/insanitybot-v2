using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using static System.Convert;

namespace InsanityBot.Commands.Moderation
{
    public class Purge : BaseCommandModule
    {
        [Command("purge")]
        [Aliases("clear")]
        public async Task PurgeCommand(CommandContext ctx,
            Int32 messageCount,

            [RemainingText]
            String arguments = "usedefault")
        {
            if(arguments.StartsWith('-'))
            {
                await this.ParsePurgeCommand(ctx, messageCount, arguments);
                return;
            }
            await this.ExecutePurgeCommand(ctx, messageCount, false, arguments);
        }

        private async Task ParsePurgeCommand(CommandContext ctx,
            Int32 messageCount,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if(!arguments.Contains("-r") && !arguments.Contains("--reason"))
                {
                    cmdArguments += " --reason usedefault";
                }

                await Parser.Default.ParseArguments<PurgeOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await this.ExecutePurgeCommand(ctx, messageCount, o.Silent, String.Join(' ', o.Reason));
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.moderation.purge.failure"],
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

        private async Task ExecutePurgeCommand(CommandContext ctx,
            Int32 messageCount,
            Boolean silent,
            String reason)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.purge"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //if silent delete command
            if(silent)
            {
                await ctx.Message.DeleteAsync();
            }

            String ModlogEmbedReason = reason switch
            {
                "usedefault" => InsanityBot.LanguageConfig["insanitybot.moderation.purge.default_reason"],
                _ => reason
            };

            DiscordEmbedBuilder tmpEmbedBuilder = null, moderationEmbedBuilder = new()
            {
                Title = "Purge",
                Color = DiscordColor.Yellow,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Messages", messageCount.ToString(), true)
                .AddField("Reason", ModlogEmbedReason, true);

            try
            {
                Byte batches = (Byte)(messageCount / 100),
                    leftover = (Byte)((messageCount % 100) + 1);

                IReadOnlyList<DiscordMessage> messageHolder = null;

                for(Byte b = 0; b < batches; b++)
                {
                    messageHolder = await ctx.Channel.GetMessagesAsync(100);
                    _ = ctx.Channel.DeleteMessagesAsync(messageHolder);
                }

                messageHolder = await ctx.Channel.GetMessagesAsync(leftover);
                _ = ctx.Channel.DeleteMessagesAsync(messageHolder);

                tmpEmbedBuilder = new DiscordEmbedBuilder
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.moderation.purge.success"],
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch(Exception e)
            {
                tmpEmbedBuilder = new DiscordEmbedBuilder
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.moderation.purge.failure"],
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
                    DiscordMessage msg = await ctx.Channel.SendMessageAsync(embed: tmpEmbedBuilder.Build());
                    Thread.Sleep(5000);
                    await msg.DeleteAsync();
                }
            }
        }
    }

    public class PurgeOptions : ModerationOptionBase
    {

    }
}
