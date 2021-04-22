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
    public partial class PermissionCommand : BaseCommandModule
    {
        public partial class UserPermissionCommand : BaseCommandModule
        {
            [Command("neutralize")]
            [Aliases("revoke", "neutral")]
            public async Task NeutralizePermissionCommand(CommandContext ctx, DiscordMember member,
                [RemainingText]
                String args)
            {
                if (args.StartsWith('-'))
                {
                    await ParseNeutralizePermission(ctx, member, args);
                    return;
                }
                await ExecuteNeutralizePermission(ctx, member, false, args);
            }

            private async Task ParseNeutralizePermission(CommandContext ctx, DiscordMember member, String args)
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
                    await ctx.Channel.SendMessageAsync(invalid.Build());
                    return;
                }

                try
                {
                    await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await ExecuteNeutralizePermission(ctx, member, o.Silent, o.Permission);
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

                    await ctx.Channel.SendMessageAsync(failed.Build());
                }
            }

            private async Task ExecuteNeutralizePermission(CommandContext ctx, DiscordMember member, Boolean silent, String permission)
            {
                if (!ctx.Member.HasPermission("insanitybot.permissions.user.neutral"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                if (silent)
                    await ctx.Message.DeleteAsync();

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
                    .AddField("User", member.Mention, true)
                    .AddField("Permission", permission, true);

                try
                {
                    InsanityBot.PermissionEngine.NeutralizeUserPermissions(member.Id, new[] { permission });

                    embedBuilder = new()
                    {
                        Color = new(0xff6347),
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        },
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_neutralized"], ctx, member, permission)
                    };

                    InsanityBot.Client.Logger.LogInformation(new EventId(9001, "Permissions"), $"Neutralized permission override {permission} from {member.Username}");
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
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_neutralize"], ctx, member)
                    };

                    InsanityBot.Client.Logger.LogCritical(new EventId(9001, "Permissions"), $"Administrative action failed: could not neutralize " +
                        $"permission override {permission} from {member.Username}. Please contact the InsanityBot team immediately\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    if (!silent)
                        await ctx.Channel.SendMessageAsync(embedBuilder.Build());

                    _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
                }
            }
        }
    }
}
