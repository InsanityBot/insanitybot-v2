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
    public partial class PermissionCommand : BaseCommandModule
    {
        public partial class RolePermissionCommand : BaseCommandModule
        {
            [Command("deny")]
            [Aliases("remove")]
            public async Task DenyPermissionCommand(CommandContext ctx, DiscordRole role,
                [RemainingText]
                String args)
            {
                if(args.StartsWith('-'))
                {
                    await this.ParseDenyPermission(ctx, role, args);
                    return;
                }
                await this.ExecuteDenyPermission(ctx, role, false, args);
            }

            private async Task ParseDenyPermission(CommandContext ctx, DiscordRole role, String args)
            {
                if(!args.Contains("-p"))
                {
                    DiscordEmbedBuilder invalid = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.permission_not_found"], ctx, role));
                    await ctx.Channel.SendMessageAsync(invalid.Build());
                    return;
                }

                try
                {
                    await Parser.Default.ParseArguments<PermissionOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await this.ExecuteDenyPermission(ctx, role, o.Silent, o.Permission);
                        });
                }
                catch(Exception e)
                {
                    DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_parse"], ctx, role));
                    InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                    await ctx.Channel.SendMessageAsync(failed.Build());
                }
            }

            private async Task ExecuteDenyPermission(CommandContext ctx, DiscordRole role, Boolean silent, String permission)
            {
                if(!ctx.Member.HasPermission("insanitybot.permissions.role.deny"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                if(silent)
                {
                    await ctx.Message.DeleteAsync();
                }

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.permissions.role.deny"];

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true)
                    .AddField("Permission", permission, true);

                try
                {
                    InsanityBot.PermissionEngine.RevokeRolePermissions(role.Id, new[] { permission });

                    embedBuilder = InsanityBot.Embeds["insanitybot.admin.permissions.role.deny"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_permission_denied"], ctx, role, permission));

                    InsanityBot.Client.Logger.LogInformation(new EventId(9012, "Permissions"), $"Denied permission {permission} for {role.Name}");
                }
                catch(Exception e)
                {
                    embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.role_could_not_deny"], ctx, role));

                    InsanityBot.Client.Logger.LogCritical(new EventId(9012, "Permissions"), $"Administrative action failed: could not deny " +
                        $"permission {permission} for {role.Name}. Please contact the InsanityBot team immediately.\n" +
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

