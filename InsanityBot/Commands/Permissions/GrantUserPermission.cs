namespace InsanityBot.Commands.Permissions;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

[Group("permission")]
[Aliases("permissions")]
public partial class PermissionCommand : BaseCommandModule
{
	public partial class UserPermissionCommand : BaseCommandModule
	{
		[Command("grant")]
		[Aliases("give", "allow")]
		[RequireAdminPermission("insanitybot.permissions.user.grant")]
		public async Task GrantPermissionCommand(CommandContext ctx, DiscordMember member,
			[RemainingText]
				String args)
		{
			if(args.StartsWith('-'))
			{
				await this.ParseGrantPermission(ctx, member, args);
				return;
			}
			await this.ExecuteGrantPermission(ctx, member, false, args);
		}

		private async Task ParseGrantPermission(CommandContext ctx, DiscordMember member, String args)
		{
			if(!args.Contains("-p"))
			{
				DiscordEmbedBuilder invalid = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_not_found"], ctx, member));

				await ctx.Channel?.SendMessageAsync(invalid.Build());
				return;
			}

			try
			{
				await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
					.WithParsedAsync(async o =>
					{
						await this.ExecuteGrantPermission(ctx, member, o.Silent, o.Permission);
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

		private async Task ExecuteGrantPermission(CommandContext ctx, DiscordMember member, Boolean silent, String permission)
		{
			if(silent)
			{
				await ctx.Message?.DeleteAsync();
			}

			DiscordEmbedBuilder embedBuilder = null;
			DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.permissions.user.grant"];

			moderationEmbedBuilder.AddField("Administrator", ctx.Member?.Mention, true)
				.AddField("User", member.Mention, true)
				.AddField("Permission", permission, true);

			try
			{
				InsanityBot.PermissionEngine.GrantUserPermissions(member.Id, new[] { permission });

				embedBuilder = InsanityBot.Embeds["insanitybot.admin.permissions.user.grant"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_granted"], ctx, member, permission));

				InsanityBot.Client.Logger.LogInformation(new EventId(9000, "Permissions"), $"Added permission {permission} to {member.Username}");
			}
			catch(Exception e)
			{
				embedBuilder = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_grant"], ctx, member));

				InsanityBot.Client.Logger.LogCritical(new EventId(9000, "Permissions"), $"Administrative action failed: could not grant " +
					$"permission {permission} to {member.Username}. Please contact the InsanityBot team immediately.\n" +
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
					Embed = moderationEmbedBuilder
				}, ctx);
			}
		}
	}
}