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

public class BlacklistSlashCommand : ApplicationCommandModule
{
	[SlashCommand("blacklist", "Blacklists the selected user.")]
	public async Task BlacklistCommand(InteractionContext ctx,

		[Option("target", "The selected user.")]
			DiscordUser user,

		[Option("reason", "The modlog reason for this action.")]
			String reason = "usedefault",

		[Option("silent", "Defines whether or not this action should be performed silently.")]
			Boolean silent = false)
	{
		if(!ctx.Member.HasPermission("insanitybot.moderation.blacklist"))
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

		String blacklistReason = reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
				ctx, member),
			_ => GetFormattedString(reason, ctx, member)
		};

		DiscordEmbedBuilder embedBuilder = null;
		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.blacklist"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", member.Mention, true)
			.AddField("Reason", blacklistReason, true);

		try
		{
			_ = member.TryAddModlogEntry(ModlogEntryType.blacklist, blacklistReason);
			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.blacklist"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.blacklist.success"], ctx, member));

			_ = member.GrantRoleAsync(InsanityBot.HomeGuild.GetRole(
				InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.blacklist_role")),
				blacklistReason);
			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder.Build()
			}, ctx);
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.blacklist.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder()
					.AddEmbed(embedBuilder));
		}
	}
}