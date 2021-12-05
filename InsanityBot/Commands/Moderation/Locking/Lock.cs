namespace InsanityBot.Commands.Moderation.Locking;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public class Lock : BaseCommandModule
{
	[Command("lock")]
	[RequirePermission("insanitybot.moderation.lock")]
	public async Task LockCommand(CommandContext ctx) => await this.LockCommand(ctx, ctx.Channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);

	[Command("lock")]
	public async Task LockCommand(CommandContext ctx, DiscordChannel channel) => await this.LockCommand(ctx, channel, InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"], false);

	[Command("lock")]
	public async Task LockCommand(CommandContext ctx, String args)
	{
		try
		{
			String cmdArguments = args;

			if(!args.Contains("-r") && !args.Contains("--reason"))
			{
				cmdArguments += " --reason usedefault";
			}

			await Parser.Default.ParseArguments<LockOptions>(cmdArguments.Split(' '))
				.WithParsedAsync(async o =>
				{
					await this.LockCommand(ctx, InsanityBot.HomeGuild.GetChannel(o.ChannelId), String.Join(' ', o.Reason), o.Silent);
				});
		}
		catch(Exception e)
		{
			DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.lock.failure"], ctx));
			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

			await ctx.Channel?.SendMessageAsync(embed: failed.Build());
		}
	}

	[Command("lock")]
	public async Task LockCommand(CommandContext ctx, DiscordChannel channel, String args) => await this.LockCommand(ctx, args + $" -c {channel.Id}");

	private async Task LockCommand(CommandContext ctx, DiscordChannel channel, String reason = "usedefault", Boolean silent = false)
	{
		String LockReason = reason switch
		{
			"usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
							ctx),
			_ => GetFormattedString(reason, ctx)
		};

		DiscordEmbedBuilder embedBuilder = null;
		DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.lock"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Channel", channel.Mention, true)
			.AddField("Reason", LockReason, true);

		try
		{
			channel.SerializeChannelData();
			ChannelData data = channel.GetCachedChannelData();

			await channel.AddOverwriteAsync(InsanityBot.HomeGuild.EveryoneRole, deny: DSharpPlus.Permissions.SendMessages, reason: "InsanityBot - locking channel");

			foreach(UInt64 v in data.LockedRoles)
			{
				await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), deny: DSharpPlus.Permissions.SendMessages, reason: "InsanityBot - locking channel, removing access for listed roles");
			}

			foreach(UInt64 v in data.WhitelistedRoles)
			{
				await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v), allow: DSharpPlus.Permissions.SendMessages, reason: "InsanityBot - locking channel, re-adding access for whitelisted roles");
			}

			UInt64 exemptRole;
			if((exemptRole = InsanityBot.Config.Value<UInt64>("insanitybot.identifiers.moderation.lock_exempt_role_id")) != 0)
			{
				await channel.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(exemptRole), allow: DSharpPlus.Permissions.SendMessages, reason:
					"InsanityBot - locking channel, granting access to whitelisted users");
			}

			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.lock"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.lock.success"], ctx));
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.lock.failure"], ctx));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			if(!silent)
			{
				await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
			}
		}
	}
}

public class LockOptions : ModerationOptionBase
{
	[Option('c', "channel", Default = 0, Required = false)]
	public UInt64 ChannelId { get; set; }
}