﻿namespace InsanityBot.SlashCommands.Moderation;
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

public class MuteSlashCommand : ApplicationCommandModule
{
	[SlashCommand("mute", "Mutes an user")]
	public async Task MuteCommand(InteractionContext ctx,

		[Option("member", "Mentioned member to mute")]
			DiscordUser user,

		[Option("reason", "Mute reason for this action")]
			String reason = "usedefault",

		[Option("silent", "Keeps the mute silent")]
			Boolean silent = false)
	{
		if(!ctx.Member.HasPermission("insanitybot.moderation.mute"))
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

		String muteReason = reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
				ctx, member),
			_ => GetFormattedString(reason, ctx, member)
		};

		DiscordEmbedBuilder embedBuilder = null;
		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.mute"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", member.Mention, true)
			.AddField("Reason", muteReason, true);

		try
		{
			_ = member.TryAddModlogEntry(ModlogEntryType.mute, muteReason);
			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.mute"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.mute.success"], ctx, member));

			await member.GrantRoleAsync(InsanityBot.HomeGuild.GetRole(
				InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.mute_role")),
				muteReason);

			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder
			}, ctx);
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.mute.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder()
					.AddEmbed(embedBuilder));
		}
	}
}