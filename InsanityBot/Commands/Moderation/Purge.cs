using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InsanityBot.Commands.Moderation
{
    public class Purge : BaseCommandModule
    {
        [Command("purge")]
        [Aliases("clear")]
        public async Task PurgeCommand(CommandContext ctx,
            UInt64 messageCount,

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
            UInt64 messageCount,
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
                DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.moderation.purge.failure"]);

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel.SendMessageAsync(embed: failed.Build());
            }
        }

        private async Task ExecutePurgeCommand(CommandContext ctx,
            UInt64 messageCount,
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

            DiscordEmbedBuilder tmpEmbedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.purge"];

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Messages", messageCount.ToString(), true)
                .AddField("Reason", ModlogEmbedReason, true);

            try
            {
                // its small enough to be a message count
                if(messageCount < (UInt64)Int16.MaxValue)
                {
                    Byte batches = (Byte)(messageCount / 100),
                        leftover = (Byte)((messageCount % 100) + 1);

                    IReadOnlyList<DiscordMessage> messageBuffer = null;

                    for(Byte b = 0; b < batches; b++)
                    {
                        messageBuffer = await ctx.Channel.GetMessagesAsync(100);
                        _ = ctx.Channel.DeleteMessagesAsync(messageBuffer);
                    }

                    messageBuffer = await ctx.Channel.GetMessagesAsync(leftover);
                    _ = ctx.Channel.DeleteMessagesAsync(messageBuffer);

                    tmpEmbedBuilder = InsanityBot.Embeds["insanitybot.moderation.purge"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.moderation.purge.success"]);

                    _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Moderation, new DiscordMessageBuilder
                    {
                        Embed = moderationEmbedBuilder
                    });
                }
                else // its a discord snowflake
                {
                    DiscordMessage message = await ctx.Channel.GetMessageAsync(messageCount);

                    if(message == null)
                    {
                        DiscordEmbedBuilder error = InsanityBot.Embeds["insanitybot.error"]
                            .WithDescription("Invalid message snowflake.");

                        await ctx.Channel.SendMessageAsync(error.Build());
                        return;
                    }

                    IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAfterAsync(messageCount, Int16.MaxValue);
                    _ = ctx.Channel.DeleteMessagesAsync(messages);

                    tmpEmbedBuilder = InsanityBot.Embeds["insanitybot.moderation.purge"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.moderation.purge.success"]);

                    _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Moderation, new DiscordMessageBuilder
                    {
                        Embed = moderationEmbedBuilder
                    });
                }
            }
            catch(Exception e)
            {
                tmpEmbedBuilder = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.moderation.purge.failure"]);
                
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
