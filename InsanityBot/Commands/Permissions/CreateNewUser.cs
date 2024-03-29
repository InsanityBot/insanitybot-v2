﻿namespace InsanityBot.Commands.Permissions;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public partial class PermissionCommand
{
	public partial class UserPermissionCommand
	{
		[Command("create")]
		[RequireAdminPermission("insanitybot.permissions.user.create")]
		public async Task CreatePermissionCommand(CommandContext ctx, DiscordMember member,
			[RemainingText]
				String args = "void")
		{
			if(args.StartsWith('-'))
			{
				await this.ParseCreatePermission(ctx, member, args);
				return;
			}
			await this.ExecuteCreatePermission(ctx, member, false);
		}

		private async Task ParseCreatePermission(CommandContext ctx, DiscordMember member, String args)
		{
			try
			{
				await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
					.WithParsedAsync(async o =>
					{
						await this.ExecuteCreatePermission(ctx, member, o.Silent);
					});
			}
			catch(Exception e)
			{
				DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_parse"], ctx, member));

				InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

				await ctx.Channel?.SendMessageAsync(failed.Build());
			}
		}

		private async Task ExecuteCreatePermission(CommandContext ctx, DiscordMember member, Boolean silent)
		{
			if(silent)
			{
				await ctx.Message?.DeleteAsync();
			}

			DiscordEmbedBuilder embedBuilder = null;
			DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.permissions.user.create"];

			moderationEmbedBuilder.AddField("Administrator", ctx.Member?.Mention, true)
				.AddField("User", member.Mention, true);

			try
			{
				InsanityBot.PermissionEngine.CreateUserPermissions(member.Id);

				embedBuilder = InsanityBot.Embeds["insanitybot.admin.permissions.user.create"];
				embedBuilder.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.user_created"], ctx, member);

				InsanityBot.Client.Logger.LogInformation(new EventId(9004, "Permissions"), $"Created permission file for {member.Username}");
			}
			catch(Exception e)
			{
				embedBuilder = new()
				{
					Color = new(0xff6347),
					Footer = new()
					{
						Text = "InsanityBot 2020-2021"
					},
					Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_create"], ctx, member)
				};

				InsanityBot.Client.Logger.LogCritical(new EventId(9004, "Permissions"), $"Administrative action failed: could not create " +
					$"permission file for {member.Username}. Please contact the InsanityBot team immediately.\n" +
					$"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
			}
			finally
			{
				if(!silent)
				{
					await ctx.Channel?.SendMessageAsync(embedBuilder.Build());
				}

				_ = InsanityBot.MessageLogger.LogMessage(new DiscordMessageBuilder
				{
					Embed = moderationEmbedBuilder.Build()
				}, ctx);
			}
		}
	}
}