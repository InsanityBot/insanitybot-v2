namespace InsanityBot.Commands.Moderation.Modlog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Utility.Modlogs.Reference;
using global::InsanityBot.Utility.Modlogs.SafeAccessInterface;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public partial class Modlog
{
	[Command("verballog")]
	[RequirePermission("insanitybot.moderation.verballog")]
	public async Task VerbalLogCommand(CommandContext ctx,
		DiscordMember user)
	{
		try
		{
			_ = user.TryFetchModlog(out UserModlog modlog);

			DiscordEmbedBuilder modlogEmbed = null;

			if(modlog.VerbalLog.Count == 0)
			{
				modlogEmbed = InsanityBot.Embeds["insanitybot.verballog.empty"];
				modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.empty_modlog"],
					ctx, user);
				_ = ctx.Channel?.SendMessageAsync(embed: modlogEmbed.Build());
			}
			else
			{
				modlogEmbed = InsanityBot.Embeds["insanitybot.verballog.entries"];

				if(!InsanityBot.Config.Value<Boolean>("insanitybot.commands.modlog.allow_verballog_scrolling"))
				{
					modlogEmbed.Description = user.CreateVerballogDescription();

					await ctx.Channel?.SendMessageAsync(embed: modlogEmbed.Build());
				}
				else
				{
					String embedDescription = user.CreateVerballogDescription();

					IEnumerable<DSharpPlus.Interactivity.Page> pages = InsanityBot.Interactivity.GeneratePagesInEmbed(embedDescription, SplitType.Line, modlogEmbed);

					await ctx.Channel?.SendPaginatedMessageAsync(ctx.Member, pages);
				}
			}
		}
		catch(Exception e)
		{
			InsanityBot.Client.Logger.LogError(new EventId(1171, "VerbalLog"), $"Could not retrieve verbal logs: {e}: {e.Message}");

			DiscordEmbedBuilder failedModlog = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.failed"], ctx, user));

			await ctx.Channel?.SendMessageAsync(embed: failedModlog.Build());
		}
	}

	[Command("verballog")]
	public async Task VerbalLogCommand(CommandContext ctx)
		=> await this.VerbalLogCommand(ctx, ctx.Member);
}