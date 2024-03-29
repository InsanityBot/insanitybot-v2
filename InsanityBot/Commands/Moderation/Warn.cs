﻿namespace InsanityBot.Commands.Moderation;
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

public partial class Warn : BaseCommandModule
{
	[Command("warn")]
	[RequirePermission("insanitybot.moderation.warn")]
	public async Task WarnCommand(CommandContext ctx,
		DiscordMember target,

		[RemainingText]
			String arguments = "usedefault")
	{
		if(arguments.StartsWith('-'))
		{
			await this.ParseWarnCommand(ctx, target, arguments);
			return;
		}
		await this.ExecuteWarn(ctx, target, arguments, false, false);
	}

	private async Task ParseWarnCommand(CommandContext ctx, DiscordMember target, String arguments)
	{
		String cmdArguments = arguments;
		try
		{
			if(!arguments.Contains("-r") && !arguments.Contains("--reason"))
			{
				cmdArguments += " --reason usedefault";
			}

			await Parser.Default.ParseArguments<WarnOptions>(cmdArguments.Split(' '))
				.WithParsedAsync(async o =>
				{
					await this.ExecuteWarn(ctx, target, String.Join(' ', o.Reason), o.Silent, o.DmMember);
				});
		}
		catch(Exception e)
		{
			DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"], ctx, target));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

			await ctx.Channel?.SendMessageAsync(embed: failed.Build());
		}
	}

	private async Task ExecuteWarn(CommandContext ctx,
		DiscordMember target,
		String reason,
		Boolean silent,
		Boolean dmMember)
	{
		//if silent - delete the warn command
		if(silent)
		{
			await ctx.Message?.DeleteAsync();
		}

		//actually do something with the usedefault value
		String WarnReason = reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
							ctx, target),
			_ => GetFormattedString(reason, ctx, target)
		};

		DiscordEmbedBuilder embedBuilder = null;

		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.warn"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", target.Mention, true)
			.AddField("Reason", WarnReason, true);

		try
		{
			_ = target.TryAddModlogEntry(ModlogEntryType.warn, WarnReason);
			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.warn"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.success"], ctx, target));

			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder
			}, ctx);
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"], ctx, target));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			if(!silent)
			{
				await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
			}

			if(dmMember)
			{
				embedBuilder.Description = GetReason(InsanityBot.LanguageConfig["insanitybot.moderation.warn.reason"], WarnReason);
				await (await target.CreateDmChannelAsync())?.SendMessageAsync(embed: embedBuilder.Build());
			}
		}
	}
}


public class WarnOptions : ModerationOptionBase
{

}