using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static System.Convert;
using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Permissions
{
	public partial class PermissionCommand : BaseCommandModule
	{
		public partial class RolePermissionCommand : BaseCommandModule
		{
			[Command("neutralize")]
			[Aliases("revoke", "neutral")]
			public async Task NeutralizePermissionCommand(CommandContext ctx, DiscordRole role,
				[RemainingText]
				String args)
			{
				if (args.StartsWith('-'))
				{
					await this.ParseNeutralizePermission(ctx, role, args);
					return;
				}
				await this.ExecuteNeutralizePermission(ctx, role, false, args);
			}

			private async Task ParseNeutralizePermission(CommandContext ctx, DiscordRole role, String args)
			{
				if (!args.Contains("-p"))
				{
					DiscordEmbedBuilder invalid = new()
					{
						Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_not_found"],
							ctx, role),
						Color = DiscordColor.Red,
						Footer = new()
						{
							Text = "InsanityBot 2020-2021"
						}
					};
					await ctx.Channel.SendMessageAsync(invalid.Build());
					return;
				}

				try
				{
					await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
						.WithParsedAsync(async o =>
						{
							await this.ExecuteNeutralizePermission(ctx, role, o.Silent, o.Permission);
						});
				}
				catch (Exception e)
				{
					DiscordEmbedBuilder failed = new()
					{
						Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permission.error.could_not_parse"],
							ctx, role),
						Color = DiscordColor.Red,
						Footer = new()
						{
							Text = "InsanityBot 2020-2021"
						}
					};
					InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

					await ctx.Channel.SendMessageAsync(failed.Build());
				}
			}

			private async Task ExecuteNeutralizePermission(CommandContext ctx, DiscordRole role, Boolean silent, String permission)
			{
				if (!ctx.Member.HasPermission("insanitybot.permissions.role.neutral"))
				{
					await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
					return;
				}

				if (silent)
				{
					await ctx.Message.DeleteAsync();
				}

				DiscordEmbedBuilder embedBuilder = null;
				DiscordEmbedBuilder moderationEmbedBuilder = new()
				{
					Title = "ADMIN: Permission Neutralized",
					Color = new(0xff6347),
					Footer = new()
					{
						Text = "InsanityBot 2020-2021"
					}
				};

				moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
					.AddField("Role", role.Mention, true)
					.AddField("Permission", permission, true);

				try
				{
					InsanityBot.PermissionEngine.NeutralizeRolePermissions(role.Id, new[] { permission });

					embedBuilder = new()
					{
						Color = new(0xff6347),
						Footer = new()
						{
							Text = "InsanityBot 2020-2021"
						},
						Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_permission_neutralized"], ctx, role, permission)
					};

					InsanityBot.Client.Logger.LogInformation(new EventId(9011, "Permissions"), $"Neutralized permission override {permission} from {role.Name}");
				}
				catch (Exception e)
				{
					embedBuilder = new()
					{
						Color = new(0xff6347),
						Footer = new()
						{
							Text = "InsanityBot 2020-2021"
						},
						Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.role_could_not_neutralize"], ctx, role)
					};

					InsanityBot.Client.Logger.LogCritical(new EventId(9011, "Permissions"), $"Administrative action failed: could not neutralize " +
						$"permission override {permission} from {role.Name}. Please contact the InsanityBot team immediately\n" +
						$"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
				}
				finally
				{
					if (!silent)
					{
						await ctx.Channel.SendMessageAsync(embedBuilder.Build());
					}

					_ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
					.SendMessageAsync(embed: moderationEmbedBuilder.Build());
				}
			}
		}
	}
}

