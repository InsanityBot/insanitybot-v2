using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

namespace InsanityBot.SlashCommands.Moderation
{
    public class PurgeSlashCommand : ApplicationCommandModule
    {
        [SlashCommand("purge", "Purges all marked messages")]
        public async Task BlacklistCommand(InteractionContext ctx,

            [Option("messages", "Either the amount of messages or the ID of the last message in this channel to keep.")]
            Int64 messageCount,

            [Option("silent", "Defines whether or not this action should be performed silently.")]
            Boolean silent = true)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.purge"))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .AddEmbed(InsanityBot.Embeds["insanitybot.lacking_permission"]
                            .WithDescription(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"])
                            .Build())
                        .AsEphemeral(true));
                return;
            }

            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .AsEphemeral(silent));

            DiscordEmbedBuilder embedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.purge"];

            moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
                .AddField("Channel", ctx.Channel.Mention, true);

            try
            {
                // its small enough to be a message count
                if(messageCount < Int16.MaxValue)
                {
                    Byte batches = (Byte)(messageCount / 100),
                        leftover = (Byte)((messageCount % 100) + 1);

                    IReadOnlyList<DiscordMessage> messageBuffer = null;

                    for(Byte b = 0; b < batches; b++)
                    {
                        messageBuffer = await ctx.Channel?.GetMessagesAsync(100);
                        _ = ctx.Channel?.DeleteMessagesAsync(messageBuffer);
                    }

                    messageBuffer = await ctx.Channel?.GetMessagesAsync(leftover);
                    _ = ctx.Channel?.DeleteMessagesAsync(messageBuffer);

                    embedBuilder = InsanityBot.Embeds["insanitybot.moderation.purge"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.moderation.purge.success"]);

                    moderationEmbedBuilder.AddField("Messages", messageCount.ToString(), true);

                    _ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
                    {
                        Embed = moderationEmbedBuilder
                    }, ctx);
                }
                else // its a discord snowflake
                {
                    DiscordMessage message = await ctx.Channel?.GetMessageAsync(Convert.ToUInt64(messageCount));

                    if(message == null)
                    {
                        DiscordEmbedBuilder error = InsanityBot.Embeds["insanitybot.error"]
                            .WithDescription("Invalid message snowflake.");

                        await ctx.Channel?.SendMessageAsync(error.Build());
                        return;
                    }

                    IReadOnlyList<DiscordMessage> messages = await ctx.Channel?.GetMessagesAfterAsync(Convert.ToUInt64(messageCount), Int16.MaxValue);
                    _ = ctx.Channel?.DeleteMessagesAsync(messages);

                    embedBuilder = InsanityBot.Embeds["insanitybot.moderation.purge"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.moderation.purge.success"]);

                    moderationEmbedBuilder.AddField("Messages", messages.Count.ToString(), true);

                    _ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
                    {
                        Embed = moderationEmbedBuilder
                    }, ctx);
                }
            }
            catch(Exception e)
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.moderation.purge.failure"]);

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .AddEmbed(embedBuilder));
            }
        }
    }
}