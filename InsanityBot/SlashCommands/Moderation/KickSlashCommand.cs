using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Modlogs.SafeAccessInterface;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.SlashCommands.StringUtilities;

namespace InsanityBot.SlashCommands.Moderation
{
    public class KickSlashCommand : ApplicationCommandModule
    {
        [SlashCommand("kick", "Kicks the selected user.")]
        public async Task KickCommand(InteractionContext ctx,

            [Option("target", "The selected user.")]
            DiscordUser user,

            [Option("reason", "The modlog reason for this action.")]
            String reason = "usedefault",

            [Option("silent", "Defines whether or not this action should be performed silently.")]
            Boolean silent = false)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.kick"))
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

            DiscordMember member = await InsanityBot.HomeGuild.GetMemberAsync(user.Id);

            String kickReason = reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                    ctx, member),
                _ => GetFormattedString(reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;
            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.kick"];

            moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", kickReason, true);

            try
            {
                _ = member.TryAddModlogEntry(ModlogEntryType.kick, kickReason);
                embedBuilder = InsanityBot.Embeds["insanitybot.moderation.kick"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.kick.success"], ctx, member));

                _ = member.RemoveAsync(kickReason);
                _ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder
                }, ctx);
            }
            catch(Exception e)
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.kick.failure"],
                        ctx, member));

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