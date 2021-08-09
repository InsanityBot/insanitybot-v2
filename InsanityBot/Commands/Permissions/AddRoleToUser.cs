using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Permissions;
using InsanityBot.Utility.Permissions.Data;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Permissions
{
    public partial class PermissionCommand : BaseCommandModule
    {
        public partial class UserPermissionCommand : BaseCommandModule
        {
            [Command("addrole")]
            [Aliases("add-role")]
            public async Task AddRoleCommand(CommandContext ctx, DiscordMember member,
                [RemainingText]
                String args)
            {
                if(args.StartsWith('-'))
                {
                    await this.ParseAddRole(ctx, member, args);
                    return;
                }
                await this.ExecuteAddRole(ctx, member, false, Convert.ToUInt64(args));
            }

            private async Task ParseAddRole(CommandContext ctx, DiscordMember member, String args)
            {
                if(!args.Contains("-r"))
                {
                    DiscordEmbedBuilder invalid = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_not_found"], ctx, member));

                    await ctx.Channel.SendMessageAsync(invalid.Build());
                    return;
                }

                try
                {
                    await Parser.Default.ParseArguments<RoleOptions>(args.Split(' '))
                        .WithParsedAsync(async o =>
                        {
                            await this.ExecuteAddRole(ctx, member, o.Silent, o.RoleId);
                        });
                }
                catch(Exception e)
                {
                    DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_parse"], ctx, member));
                    InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                    await ctx.Channel.SendMessageAsync(failed.Build());
                }
            }

            private async Task ExecuteAddRole(CommandContext ctx, DiscordMember member, Boolean silent, UInt64 role)
            {
                if(!ctx.Member.HasPermission("insanitybot.permissions.user.add_role"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                if(silent)
                {
                    await ctx.Message.DeleteAsync();
                }

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.permissions.user.addrole"];

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("User", member.Mention, true)
                    .AddField("Role ID", role.ToString(), true)
                    .AddField("Role", InsanityBot.HomeGuild.GetRole(role).Mention, true);

                try
                {
                    UserPermissions permissions = InsanityBot.PermissionEngine.GetUserPermissions(member.Id);
                    if(!permissions.AssignedRoles.Contains(role))
                    {
                        permissions.AssignedRoles = permissions.AssignedRoles.Append(role).ToArray();
                    }

                    InsanityBot.PermissionEngine.SetUserPermissions(permissions);

                    embedBuilder = InsanityBot.Embeds["insanitybot.admin.permissions.user.addrole"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.role_added"], ctx, member, 
                            InsanityBot.HomeGuild.GetRole(role)));

                    InsanityBot.Client.Logger.LogInformation(new EventId(9003, "Permissions"), $"Added role {role} to {member.Username}");
                }
                catch(Exception e)
                {
                    embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.permissions.error.could_not_add_role"], ctx, member, 
                            InsanityBot.HomeGuild.GetRole(role)));

                    InsanityBot.Client.Logger.LogCritical(new EventId(9003, "Permissions"), $"Administrative action failed: could not add " +
                        $"role {role} to {member.Username}. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}:{e.Message}\n{e.StackTrace}");
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
