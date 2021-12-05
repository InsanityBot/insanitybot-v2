namespace InsanityBot.Commands.Miscellaneous;
using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Core.Formatters.Embeds;

public class Embed : BaseCommandModule
{
	[Command("embed")]
	[RequirePermission("insanitybot.miscellaneous.say.embed")]
	public async Task SayEmbedCommand(CommandContext ctx,
		[RemainingText]
			String text)
	{
		await ctx.Message?.DeleteAsync();
		_ = ctx.Channel?.SendMessageAsync((InsanityBot.EmbedFactory.GetFormatter() as EmbedFormatter).Read(text));
	}
}