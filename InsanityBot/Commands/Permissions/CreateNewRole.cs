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
    public partial class PermissionCommand
    {
        public partial class RolePermissionCommand
        {
            [Command("create")]
            public async Task CreatePermissionCommand(CommandContext ctx, DiscordRole role,
                [RemainingText]
                String args = "void")
            {
                if(args.StartsWith('-'))
                {
                    await ParseCreatePermission(ctx, role, args);
                    return;
                }
                await ExecuteCreatePermission(ctx, role, false);
            }

            private async Task ParseCreatePermission(CommandContext ctx, DiscordRole role, String args)
            {
                try
                {
                    await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await ExecuteCreatePermission(ctx, role, o.Silent);
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

                    await ctx.Channel.SendMessageAsync(failed.Build());
                }
            }

            private async Task ExecuteCreatePermission(CommandContext ctx, DiscordRole role, Boolean silent)
            {
                if(!ctx.Member.HasPermission("insanitybot.permissions.role.create"))
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
                    Title = "ADMIN: Permission File Created",
                    Color = new(0xff6347),
                    Footer = new()
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true);

                try
                {
                    InsanityBot.PermissionEngine.CreateRolePermissions(role.Id);

                    embedBuilder = new()
                    {
                        Color = new(0xff6347),
                        Footer = new()
                        {
                            Text = "InsanityBot 2020-2021"
                        },
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_created"], ctx, role)
                    };

                    InsanityBot.Client.Logger.LogInformation(new EventId(9014, "Permissions"), $"Created permission file for {role.Name}");
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
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.role_could_not_create"], ctx, role)
                    };

                    InsanityBot.Client.Logger.LogCritical(new EventId(9014, "Permissions"), $"Administrative action failed: could not create " +
                        $"permission file for {role.Name}. Please contact the InsanityBot team immediately.\n" +
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
                        Embed = moderationEmbedBuilder.Build()
                    });
                }
            }
        }
    }
}
