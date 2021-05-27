using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;
using InsanityBot.Utility.Permissions.Data;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Permissions
{
    public partial class PermissionCommand : BaseCommandModule
    {
        [Group("role")]
        public partial class RolePermissionCommand : BaseCommandModule
        {
            [Command("addrole")]
            [Aliases("add-role")]
            public async Task AddRoleCommand(CommandContext ctx, DiscordRole role,
                [RemainingText]
                String args)
            {
                if (args.StartsWith('-'))
                {
                    await ParseAddRole(ctx, role, args);
                    return;
                }
                await ExecuteAddRole(ctx, role, false, Convert.ToUInt64(args));
            }

            private async Task ParseAddRole(CommandContext ctx, DiscordRole role, String args)
            {
                if (!args.Contains("-r"))
                {
                    DiscordEmbedBuilder invalid = new()
                    {
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_not_found"],
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
                    await Parser.Default.ParseArguments<RoleOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await ExecuteAddRole(ctx, role, o.Silent, o.RoleId);
                        });
                }
                catch (Exception e)
                {
                    DiscordEmbedBuilder failed = new()
                    {
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_parse"],
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

            private async Task ExecuteAddRole(CommandContext ctx, DiscordRole role, Boolean silent, UInt64 parent)
            {
                if (!ctx.Member.HasPermission("insanitybot.permissions.role.add_parent"))
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
                    Title = "ADMIN: Add parent to role",
                    Color = new(0xff6347),
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true)
                    .AddField("Parent ID", parent.ToString(), true)
                    .AddField("Parent", InsanityBot.HomeGuild.GetRole(parent).Mention, true);

                try
                {
                    RolePermissions permissions = InsanityBot.PermissionEngine.GetRolePermissions(role.Id);
                    permissions.Parent = parent;
                    InsanityBot.PermissionEngine.SetRolePermissions(permissions);

                    embedBuilder = new()
                    {
                        Color = new(0xff6347),
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        },
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.parent_added"],
                            ctx, role, InsanityBot.HomeGuild.GetRole(parent))
                    };

                    InsanityBot.Client.Logger.LogInformation(new EventId(9003, "Permissions"), $"Added role {parent} to {role.Name}");
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
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_add_parent"],
                            ctx, role, InsanityBot.HomeGuild.GetRole(parent))
                    };

                    InsanityBot.Client.Logger.LogCritical(new EventId(9003, "Permissions"), $"Administrative action failed: could not add parent " +
                        $"role {parent} to {role.Name}. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}:{e.Message}\n{e.StackTrace}");
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
