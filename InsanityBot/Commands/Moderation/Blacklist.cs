namespace InsanityBot.Commands.Moderation;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Utility.Modlogs.Reference;
using global::InsanityBot.Utility.Modlogs.SafeAccessInterface;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public class Blacklist : BaseCommandModule
{
	[Command("blacklist")]
	[RequirePermission("insanitybot.moderation.blacklist")]
	public async Task BlacklistCommand(CommandContext ctx,
		DiscordMember member,

		[RemainingText]
			String Reason = "usedefault")
	{
		if(Reason.StartsWith('-'))
		{
			await this.ParseBlacklistCommand(ctx, member, Reason);
			return;
		}
		await this.ExecuteBlacklistCommand(ctx, member, Reason, false, false);
	}

	private async Task ParseBlacklistCommand(CommandContext ctx,
		DiscordMember member,
		String arguments)
	{
		String cmdArguments = arguments;
		try
		{
			if(!arguments.Contains("-r") && !arguments.Contains("--reason"))
			{
				cmdArguments += " --reason usedefault";
			}

			await Parser.Default.ParseArguments<BlacklistOptions>(cmdArguments.Split(' '))
				.WithParsedAsync(async o =>
				{
					await this.ExecuteBlacklistCommand(ctx, member,
						String.Join(' ', o.Reason), o.Silent, o.DmMember);
				});
		}
		catch(Exception e)
		{
			DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.blacklist.failure"], ctx, member));
			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

			await ctx.Channel?.SendMessageAsync(embed: failed.Build());
		}
	}

	private async Task ExecuteBlacklistCommand(CommandContext ctx,
		DiscordMember member,
		String Reason,
		Boolean Silent,
		Boolean DmMember)
	{
		//actually do something with the usedefault value
		String BlacklistReason = Reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
							ctx, member),
			_ => GetFormattedString(Reason, ctx, member)
		};

		DiscordEmbedBuilder embedBuilder = null;

		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.blacklist"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", member.Mention, true)
			.AddField("Reason", BlacklistReason, true);

		try
		{
			_ = member.TryAddModlogEntry(ModlogEntryType.blacklist, BlacklistReason);
			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.blacklist"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.blacklist.success"], ctx, member));

			_ = member.GrantRoleAsync(InsanityBot.HomeGuild.GetRole(
				InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.blacklist_role")),
				BlacklistReason);
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
			await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
		}
	}
}

public class BlacklistOptions : ModerationOptionBase
{

}