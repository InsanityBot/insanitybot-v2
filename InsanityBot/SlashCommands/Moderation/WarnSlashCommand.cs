namespace InsanityBot.SlashCommands.Moderation;
using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

using global::InsanityBot.Utility.Modlogs.Reference;
using global::InsanityBot.Utility.Modlogs.SafeAccessInterface;
using global::InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.SlashCommands.StringUtilities;

public class WarnSlashCommand : ApplicationCommandModule
{
	[SlashCommand("warn", "Warns the selected user.")]
	public async Task WarnCommand(InteractionContext ctx,

		[Option("target", "The selected user.")]
			DiscordUser user,

		[Option("reason", "The modlog reason for this action.")]
			String reason,

		[Option("silent", "Defines whether or not this action should be performed silently.")]
			Boolean silent = false)
	{
		if(!ctx.Member.HasPermission("insanitybot.moderation.warn"))
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

		String warnReason = reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
				ctx, member),
			_ => GetFormattedString(reason, ctx, member)
		};

		DiscordEmbedBuilder embedBuilder = null;
		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.warn"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", member.Mention, true)
			.AddField("Reason", warnReason, true);

		try
		{
			_ = member.TryAddModlogEntry(ModlogEntryType.warn, warnReason);
			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.warn"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.success"], ctx, member));

			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder
			}, ctx);
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder()
					.AddEmbed(embedBuilder));
		}
	}
}