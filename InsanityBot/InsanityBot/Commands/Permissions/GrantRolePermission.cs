using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Permissions
{
    public partial class PermissionCommand
    {
        public partial class RolePermissionCommand : BaseCommandModule
        {
            public async Task GrantPermissionCommand(CommandContext ctx, DiscordRole role,
                [RemainingText]
                String args)
            {
                if(args.StartsWith('-'))
                {
                    await ParseGrantPermission(ctx, role, args);
                    return;
                }
                await ExecuteGrantPermission(ctx, role, false, args);
            }

            private async Task ParseGrantPermission(CommandContext ctx, DiscordRole role, String args)
            {
                if(!args.Contains("-p"))
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
                    await ctx.RespondAsync(invalid.Build());
                    return;
                }

                try
                {
                    await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await ExecuteGrantPermission(ctx, role, o.Silent, o.Permission);
                        });
                }
                catch(Exception e)
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

                    await ctx.RespondAsync(failed.Build());
                }
            }

            private async Task ExecuteGrantPermission(CommandContext ctx, DiscordRole role, Boolean silent, String args)
            {
                if(!ctx.Member.HasPermission("insanitybot.permissions.role.grant"))
                {
                    await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                if (silent)
                    await ctx.Message.DeleteAsync();

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = new()
                {
                    Title = "ADMIN: Permission Granted",
                    Color = new(0xff6347),
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true)
                    .AddField("Permission", args, true);

                try
                {
                    InsanityBot.PermissionEngine.GrantRolePermissions(role.Id, new[] { args });

                    embedBuilder = new()
                    {
                        Color = new(0xff6347),
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        },
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_permission_granted"],
                            ctx, role, args)
                    };

                    InsanityBot.Client.Logger.LogInformation(new EventId(9010, "Permissions"), $"Added permission {args} to {role.Name}");
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
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.role_could_not_grant"],
                            ctx, role)
                    };

                    InsanityBot.Client.Logger.LogCritical(new EventId(9010, "Permissions"), $"Administrative action failed: could not grant " +
                        $"permission {args} to {role.Name}. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    if (!silent)
                        await ctx.RespondAsync(embedBuilder.Build());

                    _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                        .SendMessageAsync(embed: moderationEmbedBuilder.Build());
                }
            }
        }
    }
}
