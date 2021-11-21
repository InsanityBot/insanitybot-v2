namespace InsanityBot.Commands.Moderation.Modlog.Individual;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using global::InsanityBot.Utility.Modlogs.Reference;
using global::InsanityBot.Utility.Modlogs.SafeAccessInterface;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public class MuteModlog
{
	public async Task MuteModlogCommand(CommandContext ctx, DiscordUser user)
	{
		try
		{
			_ = user.TryFetchModlog(out UserModlog modlog);

			DiscordEmbedBuilder modlogEmbed = null;

			if(modlog.ModlogEntryCount == 0)
			{
				modlogEmbed = InsanityBot.Embeds["insanitybot.modlog.empty"];
				modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.empty_modlog"],
					ctx, user);
				_ = ctx.Channel?.SendMessageAsync(embed: modlogEmbed.Build());
			}
			else
			{
				modlogEmbed = InsanityBot.Embeds["insanitybot.modlog.entries"];

				if(!InsanityBot.Config.Value<Boolean>("insanitybot.commands.modlog.allow_scrolling"))
				{
					modlogEmbed.Description = user.CreateModlogDescription(ModlogEntryType.mute, false);

					await ctx.Channel?.SendMessageAsync(embed: modlogEmbed.Build());
				}
				else
				{
					String embedDescription = user.CreateModlogDescription(ModlogEntryType.mute);

					IEnumerable<Page> pages = InsanityBot.Interactivity.GeneratePagesInEmbed(embedDescription, SplitType.Line, modlogEmbed);

					await ctx.Channel?.SendPaginatedMessageAsync(ctx.Member, pages);
				}
			}
		}
		catch(Exception e)
		{
			InsanityBot.Client.Logger.LogError(new EventId(1170, "Modlog"), $"Could not retrieve modlogs: {e}: {e.Message}");

			DiscordEmbedBuilder failedModlog = InsanityBot.Embeds["insanitybot.error"]
				.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.failed"], ctx, user));

			await ctx.Channel?.SendMessageAsync(embed: failedModlog.Build());
		}
	}
}