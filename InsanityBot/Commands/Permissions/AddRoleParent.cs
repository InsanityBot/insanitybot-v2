namespace InsanityBot.Commands.Permissions;
using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;
using global::InsanityBot.Utility.Permissions.Data;

using Microsoft.Extensions.Logging;

using static global::InsanityBot.Commands.StringUtilities;

public partial class PermissionCommand : BaseCommandModule
{
	[Group("role")]
	public partial class RolePermissionCommand : BaseCommandModule
	{
		[Command("addrole")]
		[Aliases("add-role")]
		[RequireAdminPermission("insanitybot.permissions.role.add_parent")]
		public async Task AddRoleCommand(CommandContext ctx, DiscordRole role,
			[RemainingText]
				String args)
		{
			if(args.StartsWith('-'))
			{
				await this.ParseAddRole(ctx, role, args);
				return;
			}
			await this.ExecuteAddRole(ctx, role, false, Convert.ToUInt64(args));
		}

		private async Task ParseAddRole(CommandContext ctx, DiscordRole role, String args)
		{
			if(!args.Contains("-r"))
			{
				DiscordEmbedBuilder invalid = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_not_found"], ctx, role));

				await ctx.Channel?.SendMessageAsync(invalid.Build());
				return;
			}

			try
			{
				await Parser.Default.ParseArguments<RoleOptions>(args.Split(' '))
					.WithParsedAsync(async o =>
					{
						await this.ExecuteAddRole(ctx, role, o.Silent, o.RoleId);
					});
			}
			catch(Exception e)
			{
				DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_parse"], ctx, role));

				InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

				await ctx.Channel?.SendMessageAsync(failed.Build());
			}
		}

		private async Task ExecuteAddRole(CommandContext ctx, DiscordRole role, Boolean silent, UInt64 parent)
		{
			if(silent)
			{
				await ctx.Message?.DeleteAsync();
			}

			DiscordEmbedBuilder embedBuilder = null;
			DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.permissions.role.parent"];

			moderationEmbedBuilder.AddField("Administrator", ctx.Member?.Mention, true)
				.AddField("Role", role.Mention, true)
				.AddField("Parent ID", parent.ToString(), true)
				.AddField("Parent", InsanityBot.HomeGuild.GetRole(parent).Mention, true);

			try
			{
				RolePermissions permissions = InsanityBot.PermissionEngine.GetRolePermissions(role.Id);
				permissions.Parent = parent;
				InsanityBot.PermissionEngine.SetRolePermissions(permissions);

				embedBuilder = InsanityBot.Embeds["insanitybot.admin.permissions.role.parent"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.parent_added"],
						ctx, role, InsanityBot.HomeGuild.GetRole(parent)));

				InsanityBot.Client.Logger.LogInformation(new EventId(9003, "Permissions"), $"Added role {parent} to {role.Name}");
			}
			catch(Exception e)
			{
				embedBuilder = InsanityBot.Embeds["insanitybot.error"]
					.WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_add_parent"],
						ctx, role, InsanityBot.HomeGuild.GetRole(parent)));

				InsanityBot.Client.Logger.LogCritical(new EventId(9003, "Permissions"), $"Administrative action failed: could not add parent " +
					$"role {parent} to {role.Name}. Please contact the InsanityBot team immediately.\n" +
					$"Please also provide them with the following information:\n\n{e}:{e.Message}\n{e.StackTrace}");
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