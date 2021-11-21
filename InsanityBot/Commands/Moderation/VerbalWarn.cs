namespace InsanityBot.Commands.Moderation;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Utility.Modlogs; // unsafe interface to allow faster method chaining
using global::InsanityBot.Utility.Modlogs.SafeAccessInterface;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public class VerbalWarn : BaseCommandModule
{
	[Command("verbalwarn")]
	[Aliases("vw", "verbal-warn")]
	[RequirePermission("insanitybot.moderation.verbal_warn")]
	public async Task VerbalWarnCommand(CommandContext ctx,
		DiscordMember member,

		[RemainingText]
			String arguments = "usedefault")
	{
		if(!InsanityBot.Config.Value<Boolean>("insanitybot.commands.moderation.allow_minor_warns"))
		{
			return;
		}

		if(arguments.StartsWith('-'))
		{
			await this.ParseVerbalWarnCommand(ctx, member, arguments);
			return;
		}
		await this.ExecuteVerbalWarnCommand(ctx, member, arguments, false, false);
	}

	private async Task ParseVerbalWarnCommand(CommandContext ctx,
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

			await Parser.Default.ParseArguments<VerbalWarnOptions>(cmdArguments.Split(' '))
				.WithParsedAsync(async o =>
				{
					await this.ExecuteVerbalWarnCommand(ctx, member, String.Join(' ', o.Reason), o.Silent, o.DmMember);
				});
		}
		catch(Exception e)
		{
			DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.verbal_warn.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

			await ctx.Channel?.SendMessageAsync(embed: failed.Build());
		}
	}

	private async Task ExecuteVerbalWarnCommand(CommandContext ctx,
		DiscordMember member,
		String reason,
		Boolean silent,
		Boolean dmMember)
	{
		if(silent)
		{
			await ctx.Message?.DeleteAsync();
		}

		String VerbalWarnReason = reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
				ctx, member),
			_ => GetFormattedString(reason, ctx, member)
		};

		DiscordEmbedBuilder embedBuilder = null;
		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.verbalwarn"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", member.Mention, true)
			.AddField("Reason", VerbalWarnReason, true);

		try
		{
			_ = member.TryAddVerballogEntry(VerbalWarnReason);

			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.verbalwarn"]
				.WithDescription(GetMemberReason(InsanityBot.LanguageConfig["insanitybot.moderation.verbal_warn.reason"],
					VerbalWarnReason, member));

			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder
			}, ctx);
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.verbal_warn.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			await ctx.Channel?.SendMessageAsync(embedBuilder.Build());

			if(InsanityBot.Config.Value<Boolean>("insanitybot.commands.moderation.convert_minor_warns_into_full_warn"))
			{
				if((member.GetUserModlog().VerbalLogEntryCount %
					InsanityBot.Config.Value<Int64>("insanitybot.commands.moderation.minor_warns_equal_full_warn")) == 0)
				{
					await new Warn().WarnCommand(ctx, member, $"--silent --reason Too many verbal warns, count since last warn exceeded " +
						$"{InsanityBot.Config.Value<Int64>("insanitybot.commands.moderation.minor_warns_equal_full_warn")}");
				}
			}
		}
	}
}

public class VerbalWarnOptions : ModerationOptionBase
{

}