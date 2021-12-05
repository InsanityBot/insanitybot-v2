namespace InsanityBot.Commands.Miscellaneous;

using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Utility.Timers;

using Microsoft.Extensions.Logging;

using TimeSpanParserUtil;

using static global::InsanityBot.Commands.StringUtilities;

public class TimedRole : BaseCommandModule
{
	[Command("timerole")]
	[Aliases("temprole", "temp-role")]
	[RequirePermission("insanitybot.miscellaneous.timerole")]
	public async Task TimedRoleCommand(CommandContext ctx,
		DiscordMember member,
		DiscordRole role,

		[RemainingText]
		String arg)
	{
		if(!TimeSpanParser.TryParse(arg, out TimeSpan time))
		{
			DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(
					InsanityBot.LanguageConfig["insanitybot.miscellaneous.timed_role.failure_invalid_time"], ctx, member, role));
			await ctx.Channel.SendMessageAsync(failed.Build());
		}

		DiscordEmbedBuilder embedBuilder = null, moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.misclog.timedrole"];

		moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
			.AddField("Member", member.Mention, true)
			.AddField("Role", role.Mention, true);

		try
		{
			Timer callbackTimer = new(DateTimeOffset.Now.Add(time), $"temprole_{role.Id}_{member.Id}");
			moderationEmbedBuilder.AddField("Timer GUID", callbackTimer.Guid.ToString(), true);
			TimeHandler.AddTimer(callbackTimer);

			_ = member.GrantRoleAsync(role);

			embedBuilder = InsanityBot.Embeds["insanitybot.misc.timedrole"]
				.WithDescription(GetFormattedString(
					InsanityBot.LanguageConfig["insanitybot.miscellaneous.timed_role.success"], ctx, member, role));

			_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
			{
				Embed = moderationEmbedBuilder
			}, ctx);
		}
		catch
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(
					InsanityBot.LanguageConfig["insanitybot.miscellaneous.timed_role.failure"], ctx, member));
		}
		finally
		{
			if(embedBuilder == null)
			{
				InsanityBot.Client.Logger.LogError(new EventId(1030, "Timed role"),
					"Could not execute timed role command, an unknown exception occured.");
			}
			else
			{
				await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
			}
		}
	}

	public static async void RemoveRole(String identifier, Guid guid)
	{
		if(!identifier.StartsWith("temprole_"))
		{
			return;
		}

		String userId = String.Empty;

		try
		{
			File.Delete($"./cache/timers/{identifier}");

			String[] parts = identifier.Split('_');
			userId = parts[2];

			DiscordMember member = await InsanityBot.HomeGuild.GetMemberAsync(UInt64.Parse(parts[2]));
			DiscordRole role = InsanityBot.HomeGuild.GetRole(UInt64.Parse(parts[1]));

			_ = member.RevokeRoleAsync(role);

			TimeRoleCompletedEvent();
		}
		catch(Exception e)
		{
			InsanityBot.Client.Logger.LogError(new EventId(1031, "Timed role"), $"Could not unmute user {userId}");
			Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
		}
	}

	public static event TimedActionStartEventHandler TimeRoleStartingEvent;
	public static event TimedActionCompleteEventHandler TimeRoleCompletedEvent;
}
