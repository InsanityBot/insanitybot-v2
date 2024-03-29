﻿namespace InsanityBot.Commands.Moderation;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public partial class Mute
{
	[Command("unmute")]
	[RequirePermission("insanitybot.moderation.unmute")]
	public async Task UnmuteCommand(CommandContext ctx,
		DiscordMember member,
		String arguments = null)
	{
		if(arguments == null)
		{
			await this.ExecuteUnmuteCommand(ctx, member, false, false);
			return;
		}

		if(arguments.StartsWith('-'))
		{
			await this.ParseUnmuteCommand(ctx, member, arguments);
			return;
		}

		InsanityBot.Client.Logger.LogWarning(new EventId(1133, "ArgumentParser"),
			"Unmute command was called with invalid arguments, running default arguments");
		await this.ExecuteUnmuteCommand(ctx, member, false, false);
	}

	private async Task ParseUnmuteCommand(CommandContext ctx,
		DiscordMember member,
		String arguments)
	{
		String cmdArguments = arguments;
		try
		{
			if(!arguments.Contains("-r") && !arguments.Contains("--reason"))
			{
				cmdArguments += " --reason void"; //we dont need the reason but its required by the protocol
			}

			await Parser.Default.ParseArguments<UnmuteOptions>(cmdArguments.Split(' '))
				.WithParsedAsync(async o =>
				{
					await this.ExecuteUnmuteCommand(ctx, member, o.Silent, o.DmMember);
				});
		}
		catch(Exception e)
		{
			DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unmute.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"), $"{e}: {e.Message}");

			await ctx.Channel?.SendMessageAsync(embed: failed.Build());
		}
	}

	/* ctx can be null if automated is true since ctx is only used for two purposes
	 * its used to respond to the command execution, which does not happen when silent mode is enabled
	 * (silent is enforced by auto mode)
	 * and its used to verify permissions, but that check is never called when auto mode is enabled */
	private async Task ExecuteUnmuteCommand(CommandContext ctx,
		DiscordMember member,
		Boolean silent,
		Boolean dmMember,
		Boolean automated = false,
		params Object[] additionals)
	{
		if(ctx == null && silent == false)
		{
			InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"),
				"Invalid command arguments - internal error. Please report this on https://github.com/InsanityBot/InsanityBot/issues" +
				"\nInsanityBot/Commands/Moderation/Unmute.cs: argument \"silent\" cannot be false without given command context");
			return;
		}
		if(automated && !silent)
		{
			InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"),
				"Invalid command arguments - internal error. Please report this on https://github.com/InsanityBot/InsanityBot/issues" +
				"\nInsanityBot/Commands/Moderation/Unmute.cs: argument \"silent\" cannot be false for an automated unmute");
			return;
		}

		DiscordEmbedBuilder nonSilent = null;
		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.unmute"];

		if(automated)
		{
			moderationEmbedBuilder.AddField("Moderator", "InsanityBot", true);
		}
		else
		{
			moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true);
		}

		moderationEmbedBuilder.AddField("Member", member.Mention, true);

		try
		{
			if(silent)
			{
				await member.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
					InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.mute_role_id")),
					"Silent unmute");

				if(additionals != null)
				{
					for(Byte b = 0; b < additionals.Length; b++)
					{
						if(additionals[b] is String str && str == "timer_guid")
						{
							moderationEmbedBuilder.AddField("Timer Guid", ((Guid)additionals[b + 1]).ToString(), true);
						}
					}
				}
			}
			else
			{
				nonSilent = InsanityBot.Embeds["insanitybot.moderation.unmute"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unmute.success"], ctx, member));

				_ = member.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
					InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.mute_role_id")),
					"unmute");

				if(additionals.Length >= 2)
				{
					for(Byte b = 0; b <= additionals.Length; b++)
					{
						if(additionals[b] is String str && str == "timer_guid")
						{
							moderationEmbedBuilder.AddField("Timer Guid", ((Guid)additionals[b + 1]).ToString(), true);
						}
					}
				}
			}
		}
		catch(Exception e)
		{
			if(!silent)
			{
				nonSilent = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unmute.failure"], ctx, member));
			}

			InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"), $"{e}: {e.Message}");
		}
		finally
		{
			if(!silent)
			{
				_ = ctx.Channel?.SendMessageAsync(embed: nonSilent.Build());
			}

			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder
			}, ctx);
		}
	}
}

public class UnmuteOptions : ModerationOptionBase
{

}