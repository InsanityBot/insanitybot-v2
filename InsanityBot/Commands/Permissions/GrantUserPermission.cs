using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Permissions
{
    [Group("permission")]
    [Aliases("permissions")]
    public partial class PermissionCommand : BaseCommandModule
    {
        public partial class UserPermissionCommand : BaseCommandModule
        {
            [Command("grant")]
            [Aliases("give", "allow")]
            public async Task GrantPermissionCommand(CommandContext ctx, DiscordMember member,
                [RemainingText]
                String args)
            {
                if(args.StartsWith('-'))
                {
                    await ParseGrantPermission(ctx, member, args);
                    return;
                }
                await ExecuteGrantPermission(ctx, member, false, args);
            }

            private async Task ParseGrantPermission(CommandContext ctx, DiscordMember member, String args)
            {
                if(!args.Contains("-p"))
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
                    await ctx.Channel.SendMessageAsync(invalid.Build());
                    return;
                }

                try
                {
                    await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await ExecuteGrantPermission(ctx, member, o.Silent, o.Permission);
                        });
                }
                catch(Exception e)
                {
                    DiscordEmbedBuilder failed = new()
                    {
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_parse"],
                            ctx, member),
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

            private async Task ExecuteGrantPermission(CommandContext ctx, DiscordMember member, Boolean silent, String permission)
            {
                if(!ctx.Member.HasPermission("insanitybot.permissions.user.grant"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                if(silent)
                {
                    await ctx.Message.DeleteAsync();
                }

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
                    .AddField("User", member.Mention, true)
                    .AddField("Permission", permission, true);

                try
                {
                    InsanityBot.PermissionEngine.GrantUserPermissions(member.Id, new[] { permission });

                    embedBuilder = new()
                    {
                        Color = new(0xff6347),
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        },
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_granted"], ctx, member, permission)
                    };

                    InsanityBot.Client.Logger.LogInformation(new EventId(9000, "Permissions"), $"Added permission {permission} to {member.Username}");
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
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_grant"], ctx, member)
                    };

                    InsanityBot.Client.Logger.LogCritical(new EventId(9000, "Permissions"), $"Administrative action failed: could not grant " +
                        $"permission {permission} to {member.Username}. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    if(!silent)
                    {
                        await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                    }

                    _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Administration, new DiscordMessageBuilder
                    {
                        Embed = moderationEmbedBuilder
                    });
                }
            }
        }
    }
}
