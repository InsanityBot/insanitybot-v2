namespace InsanityBot.Commands.Moderation;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public class Whitelist : BaseCommandModule
{
	[Command("whitelist")]
	[RequirePermission("insanitybot.moderation.whitelist")]
	public async Task WhitelistCommand(CommandContext ctx,
		DiscordMember member,

		[RemainingText]
			String Reason = "usedefault")
	{
		if(Reason.StartsWith('-'))
		{
			await this.ParseWhitelistCommand(ctx, member, Reason);
			return;
		}
		await this.ExecuteWhitelistCommand(ctx, member, Reason, false, false);
	}

	private async Task ParseWhitelistCommand(CommandContext ctx,
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

			await Parser.Default.ParseArguments<WhitelistOptions>(cmdArguments.Split(' '))
				.WithParsedAsync(async o =>
				{
					await this.ExecuteWhitelistCommand(ctx, member, String.Join(' ', o.Reason), o.Silent, o.DmMember);
				});
		}
		catch(Exception e)
		{
			DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

			await ctx.Channel?.SendMessageAsync(embed: failed.Build());
		}
	}

	private async Task ExecuteWhitelistCommand(CommandContext ctx,
		DiscordMember member,
		String Reason,
		Boolean Silent,
		Boolean DmMember)
	{
		//actually do something with the usedefault value
		String WhitelistReason = Reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
							ctx, member),
			_ => GetFormattedString(Reason, ctx, member)
		};

		DiscordEmbedBuilder embedBuilder = null;

		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.whitelist"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", member.Mention, true)
			.AddField("Reason", WhitelistReason, true);

		try
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.whitelist"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.success"], ctx, member));

			_ = member.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
				InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.blacklist_role_id")),
				WhitelistReason);
			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder
			}, ctx);
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
		}
	}
}

public class WhitelistOptions : ModerationOptionBase
{

}