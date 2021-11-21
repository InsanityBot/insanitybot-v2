namespace InsanityBot.Commands.Moderation.Modlog;
using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public class ExportModlog : BaseCommandModule
{
	[Command("exportmodlog")]
	[RequirePermission("insanitybot.moderation.export_modlog")]
	public async Task ExportModlogCommand(CommandContext ctx,
		DiscordMember member,
		Boolean dmFile = false)
	{
		try
		{
			DiscordChannel exportChannel;

			if(!dmFile)
			{
				exportChannel = ctx.Channel;
			}
			else
			{
				exportChannel = await ctx.Member?.CreateDmChannelAsync();
			}

			if(!File.Exists($"./data/{member.Id}/modlog.json"))
			{
				await ctx.Channel?.SendMessageAsync(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.export_modlog.no_modlog"],
					ctx, member));
				return;
			}

			Stream stream = new FileStream($"./data/{member.Id}/modlog.json", FileMode.Open);

			DiscordMessageBuilder messageBuilder = new();
			messageBuilder.WithFile("modlog_export.json", stream);

			await exportChannel?.SendMessageAsync(messageBuilder);
		}
		catch(Exception e)
		{
			InsanityBot.Client.Logger.LogError(new EventId(1181, "ExportModlog"), $"{e}: {e.Message}\n{e.StackTrace}");
		}
	}

	[Command("exportmodlog")]
	public async Task ExportModlogCommand(CommandContext ctx,
		Boolean dmFile = false) => await this.ExportModlogCommand(ctx, ctx.Member, dmFile);
}