namespace InsanityBot.Commands.Moderation;
using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Utility.Modlogs; // we're using the unsafe interface here to allow faster method chaining
using global::InsanityBot.Utility.Modlogs.Reference;
using global::InsanityBot.Utility.Modlogs.SafeAccessInterface;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public partial class Warn
{
	// WarningIndex is zero-based, guys. id highly recommend using the string command instead
	[Command("unwarn")]
	[RequirePermission("insanitybot.moderation.unwarn")]
	public async Task UnwarnCommand(CommandContext ctx,
		DiscordMember member,
		Int32 WarningIndex)
	{
		DiscordEmbedBuilder embedBuilder = null;

		try
		{
			_ = member.TryFetchModlog(out UserModlog modlog);
			modlog.Modlog.RemoveAt(WarningIndex);
			modlog.ModlogEntryCount--;
			_ = member.TrySetModlog(modlog);

			embedBuilder = InsanityBot.Embeds["insanitybot.moderation.unwarn"];
			embedBuilder.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unwarn.success"], ctx, member);
		}
		catch(Exception e)
		{
			embedBuilder = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unwarn.failure"], ctx, member));

			InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
		}
		finally
		{
			await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
		}
	}

	[Command("unwarn")]
	public async Task UnwarnCommand(CommandContext ctx,
		DiscordMember member,
		[RemainingText]
			String WarningText) => await this.UnwarnCommand(ctx, member,
			member.GetUserModlog().Modlog.IndexOf(member.GetUserModlog().Modlog.FirstOrDefault(md =>
			{
				return md.Reason.Contains(WarningText);
			})));
}