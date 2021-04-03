using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Permissions
{
    public partial class PermissionCommand : BaseCommandModule
    {
        public partial class UserPermissionCommand : BaseCommandModule
        {
            [Command("deny")]
            [Aliases("remove")]
            public async Task DenyPermissionCommand(CommandContext ctx, DiscordMember member,
                [RemainingText]
                String args)
            {
                if (args.StartsWith('-'))
                {
                    await ParseDenyPermission(ctx, member, args);
                    return;
                }
                await ExecuteDenyPermission(ctx, member, false, args);
            }

            private async Task ParseDenyPermission(CommandContext ctx, DiscordMember member, String args)
            {
                if (!args.Contains("-p"))
                {
                    DiscordEmbedBuilder invalid = new()
                    {
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_not_found"],
                            ctx, member),
                        Color = DiscordColor.Red,
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        }
                    };
                    await ctx.RespondAsync(invalid.Build());
                    return;
                }

                try
                {
                    await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await ExecuteDenyPermission(ctx, member, o.Silent, o.Permission);
                        });
                }
                catch (Exception e)
                {
                    DiscordEmbedBuilder failed = new()
                    {
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permission.error.could_not_parse"],
                            ctx, member),
                        Color = DiscordColor.Red,
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        }
                    };
                    InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                    await ctx.RespondAsync(failed.Build());
                }
            }

            private async Task ExecuteDenyPermission(CommandContext ctx, DiscordMember member, Boolean silent, String permission)
            {
                if (!ctx.Member.HasPermission("insanitybot.permissions.user.deny"))
                {
                    await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = new()
                {
                    Title = "ADMIN: Permission Deny",
                    Color = new(0xff6347),
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("User", member.Mention, true)
                    .AddField("Permission", permission, true);

                try
                {
                    InsanityBot.PermissionEngine.RevokeUserPermissions(member.Id, new[] { permission });

                    embedBuilder = new()
                    {
                        Color = new(0xff6347),
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        },
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permission.permission_denied"], ctx, member, permission)
                    };

                    InsanityBot.Client.Logger.LogInformation(new EventId(9002, "Permissions"), $"Denied permission {permission} for {member.Username}");
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
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permission.error.could_not_deny"], ctx, member)
                    };

                    InsanityBot.Client.Logger.LogCritical(new EventId(9000, "Permissions"), $"Administrative action failed: could not deny " +
                        $"permission {permission} fpr {member.Username}. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    if (!silent)
                        await ctx.RespondAsync(embedBuilder.Build());
                }
            }
        }
    }
}

