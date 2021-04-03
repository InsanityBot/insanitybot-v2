using System;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Permissions
{
    public partial class PermissionCommand
    {
        public partial class UserPermissionCommand
        {
            [Command("create")]
            public async Task CreatePermissionCommand(CommandContext ctx, DiscordMember member,
                [RemainingText]
                String args)
            {
                if (args.StartsWith('-'))
                {
                    await ParseCreatePermission(ctx, member, args);
                    return;
                }
                await ExecuteCreatePermission(ctx, member, false);
            }

            private async Task ParseCreatePermission(CommandContext ctx, DiscordMember member, String args)
            {
                try
                {
                    await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await ExecuteCreatePermission(ctx, member, o.Silent);
                        });
                }
                catch (Exception e)
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

                    await ctx.RespondAsync(failed.Build());
                }
            }

            private async Task ExecuteCreatePermission(CommandContext ctx, DiscordMember member, Boolean silent)
            {
                if (!ctx.Member.HasPermission("insanitybot.permissions.user.create"))
                {
                    await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                if (silent)
                    await ctx.Message.DeleteAsync();

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = new()
                {
                    Title = "ADMIN: Permission File Created",
                    Color = new(0xff6347),
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("User", member.Mention, true);

                try
                {
                    InsanityBot.PermissionEngine.CreateUserPermissions(member.Id);

                    embedBuilder = new()
                    {
                        Color = new(0xff6347),
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        },
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.user_created"], ctx, member)
                    };

                    InsanityBot.Client.Logger.LogInformation(new EventId(9004, "Permissions"), $"Created permission file for {member.Username}");
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
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_create"], ctx, member)
                    };

                    InsanityBot.Client.Logger.LogCritical(new EventId(9004, "Permissions"), $"Administrative action failed: could not create " +
                        $"permission file for {member.Username}. Please contact the InsanityBot team immediately.\n" +
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
