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

public partial class PermissionCommand : BaseCommandModule
{
	public partial class UserPermissionCommand : BaseCommandModule
	{
		[Command("neutralize")]
		[Aliases("revoke", "neutral", "nullify")]
		[RequireAdminPermission("insanitybot.permissions.user.neutral")]
		public async Task NeutralizePermissionCommand(CommandContext ctx, DiscordMember member,
			[RemainingText]
				String args)
		{
			if(args.StartsWith('-'))
			{
				await this.ParseNeutralizePermission(ctx, member, args);
				return;
			}
			await this.ExecuteNeutralizePermission(ctx, member, false, args);
		}

		private async Task ParseNeutralizePermission(CommandContext ctx, DiscordMember member, String args)
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
						await this.ExecuteNeutralizePermission(ctx, member, o.Silent, o.Permission);
					});
			}
			catch(Exception e)
			{
				DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permission.error.could_not_parse"], ctx, member));
				InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

				await ctx.Channel?.SendMessageAsync(failed.Build());
			}
		}

		private async Task ExecuteNeutralizePermission(CommandContext ctx, DiscordMember member, Boolean silent, String permission)
		{
			if(silent)
			{
				await ctx.Message?.DeleteAsync();
			}

			DiscordEmbedBuilder embedBuilder = null;
			DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.permissions.user.neutral"];

			moderationEmbedBuilder.AddField("Administrator", ctx.Member?.Mention, true)
				.AddField("User", member.Mention, true)
				.AddField("Permission", permission, true);

			try
			{
				InsanityBot.PermissionEngine.NeutralizeUserPermissions(member.Id, new[] { permission });

				embedBuilder = InsanityBot.Embeds["insanitybot.admin.permissions.user.neutral"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_neutralized"], ctx, member, permission));

				InsanityBot.Client.Logger.LogInformation(new EventId(9001, "Permissions"), $"Neutralized permission override {permission} from {member.Username}");
			}
			catch(Exception e)
			{
				embedBuilder = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_neutralize"], ctx, member));

				InsanityBot.Client.Logger.LogCritical(new EventId(9001, "Permissions"), $"Administrative action failed: could not neutralize " +
					$"permission override {permission} from {member.Username}. Please contact the InsanityBot team immediately\n" +
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