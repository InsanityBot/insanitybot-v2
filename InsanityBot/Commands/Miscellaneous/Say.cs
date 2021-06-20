using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

namespace InsanityBot.Commands.Miscellaneous
{
	public class Say : BaseCommandModule
	{
		[Command("say")]
		public async Task SayCommand(CommandContext ctx,
			[RemainingText]
			String text)
		{
			if (!ctx.Member.HasPermission("insanitybot.miscellaneous.say"))
			{
				await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
				return;
			}

			_ = ctx.Message.DeleteAsync();
			_ = ctx.Channel.SendMessageAsync(text);
		}

		[Command("embed")]
		public async Task SayEmbedCommand(CommandContext ctx,
			[RemainingText]
			String text)
		{
			if (!ctx.Member.HasPermission("insanitybot.miscellaneous.say.embed"))
			{
				await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
				return;
			}

			_ = ctx.Message.DeleteAsync();
			DiscordEmbedBuilder embedBuilder = new()
			{
				Description = text,
				Footer = new DiscordEmbedBuilder.EmbedFooter
				{
					Text = "InsanityBot 2020-2021"
				},
				Color = DiscordColor.Blurple
			};
			_ = ctx.Channel.SendMessageAsync(embed: embedBuilder.Build());
		}
	}
}
