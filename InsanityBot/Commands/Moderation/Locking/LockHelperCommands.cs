using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation.Locking
{
    [Group("lock-settings")]
    public class LockHelperCommands : BaseCommandModule
    {
        [Group("whitelist")]
        public class Whitelist : BaseCommandModule
        {
            [Command("add")]
            public async Task AddWhitelistedRoleCommand(CommandContext ctx, DiscordRole role) => await this.AddWhitelistedRoleCommand(ctx, role, ctx.Channel);

            [Command("add")]
            public async Task AddWhitelistedRoleCommand(CommandContext ctx, DiscordRole role, DiscordChannel channel)
            {
                if(!ctx.Member.HasPermission("insanitybot.admin.lock_whitelist.add"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.lock.whitelist"];

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true)
                    .AddField("Channel", channel.Mention, true);

                try
                {
                    ChannelData data = channel.GetCachedChannelData();

                    if(data.LockedRoles.Contains(role.Id))
                    {
                        data.LockedRoles.Remove(role.Id);
                    }

                    if(data.WhitelistedRoles.Contains(role.Id))
                    {
                        embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                            .WithDescription(GetFormattedString(
                                InsanityBot.LanguageConfig["insanitybot.commands.lock.whitelist.add.already_present"], role, channel));
                    }
                    else
                    {
                        embedBuilder = InsanityBot.Embeds["insanitybot.admin.lock.whitelist"]
                            .WithDescription(GetFormattedString(
                                InsanityBot.LanguageConfig["insanitybot.commands.lock.whitelist.add.success"], role, channel));

                        data.WhitelistedRoles.Add(role.Id);
                        channel.SerializeLockingOverwrites(data);

                        InsanityBot.Client.Logger.LogInformation(new EventId(2000, "LockAdmin"), $"Added role {role.Id} to channel whitelist");
                    }
                }
                catch(Exception e)
                {
                    embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(
                            InsanityBot.LanguageConfig["insanitybot.commands.lock.whitelist.add.failure"], role, channel));

                    InsanityBot.Client.Logger.LogCritical(new EventId(2000, "LockAdmin"),
                        $"Administrative action failed: could not add {role.Id} to channel whitelist. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                }
            }

            [Command("remove")]
            public async Task RemoveWhitelistedRoleCommand(CommandContext ctx, DiscordRole role) => await this.RemoveWhitelistedRoleCommand(ctx, role, ctx.Channel);

            [Command("remove")]
            public async Task RemoveWhitelistedRoleCommand(CommandContext ctx, DiscordRole role, DiscordChannel channel)
            {
                if(!ctx.Member.HasPermission("insanitybot.admin.lock_whitelist.remove"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.lock.unwhitelist"];

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true)
                    .AddField("Channel", channel.Mention, true);

                try
                {
                    ChannelData data = channel.GetCachedChannelData();

                    if(data.WhitelistedRoles.Contains(role.Id))
                    {
                        data.WhitelistedRoles.Remove(role.Id);
                    }

                    embedBuilder = InsanityBot.Embeds["insanitybot.admin.lock.unwhitelist"]
                        .WithDescription(GetFormattedString(
                            InsanityBot.LanguageConfig["insanitybot.commands.lock.whitelist.remove.success"], role, channel));

                    channel.SerializeLockingOverwrites(data);

                    InsanityBot.Client.Logger.LogInformation(new EventId(2000, "LockAdmin"), $"Removed role {role.Id} from channel whitelist");

                }
                catch(Exception e)
                {
                    embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(
                            InsanityBot.LanguageConfig["insanitybot.commands.lock.whitelist.remove.failure"], role, channel));

                    InsanityBot.Client.Logger.LogCritical(new EventId(2000, "LockAdmin"),
                        $"Administrative action failed: could not remove {role.Id} from channel whitelist. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                }
            }
        }

        [Group("blacklist")]
        public class Blacklist : BaseCommandModule
        {
            [Command("add")]
            public async Task AddBlacklistedRoleCommand(CommandContext ctx, DiscordRole role) => await this.AddBlacklistedRoleCommand(ctx, role, ctx.Channel);

            [Command("add")]
            public async Task AddBlacklistedRoleCommand(CommandContext ctx, DiscordRole role, DiscordChannel channel)
            {
                if(!ctx.Member.HasPermission("insanitybot.admin.lock_blacklist.add"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.lock.blacklist"];

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true)
                    .AddField("Channel", channel.Mention, true);

                try
                {
                    ChannelData data = channel.GetCachedChannelData();

                    if(data.WhitelistedRoles.Contains(role.Id))
                    {
                        data.WhitelistedRoles.Remove(role.Id);
                    }

                    if(data.LockedRoles.Contains(role.Id))
                    {
                        embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                            .WithDescription(GetFormattedString(
                                InsanityBot.LanguageConfig["insanitybot.commands.lock.blacklist.add.already_present"], role, channel));
                    }
                    else
                    {
                        embedBuilder = InsanityBot.Embeds["insanitybot.admin.lock.blacklist"]
                            .WithDescription(GetFormattedString(
                                InsanityBot.LanguageConfig["insanitybot.commands.lock.blacklist.add.success"], role, channel));

                        data.LockedRoles.Add(role.Id);
                        channel.SerializeLockingOverwrites(data);

                        InsanityBot.Client.Logger.LogInformation(new EventId(2000, "LockAdmin"), $"Added role {role.Id} to channel blacklist");
                    }
                }
                catch(Exception e)
                {
                    embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(
                            InsanityBot.LanguageConfig["insanitybot.commands.lock.blacklist.add.failure"], role, channel));

                    InsanityBot.Client.Logger.LogCritical(new EventId(2000, "LockAdmin"),
                        $"Administrative action failed: could not add {role.Id} to channel blacklist. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                }
            }

            [Command("remove")]
            public async Task RemoveBlacklistedRoleCommand(CommandContext ctx, DiscordRole role) => await this.RemoveBlacklistedRoleCommand(ctx, role, ctx.Channel);

            [Command("remove")]
            public async Task RemoveBlacklistedRoleCommand(CommandContext ctx, DiscordRole role, DiscordChannel channel)
            {
                if(!ctx.Member.HasPermission("insanitybot.admin.lock_blacklist.remove"))
                {
                    await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                    return;
                }

                DiscordEmbedBuilder embedBuilder = null;
                DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.adminlog.lock.unblacklist"];

                moderationEmbedBuilder.AddField("Administrator", ctx.Member.Mention, true)
                    .AddField("Role", role.Mention, true)
                    .AddField("Channel", channel.Mention, true);

                try
                {
                    ChannelData data = channel.GetCachedChannelData();

                    if(data.LockedRoles.Contains(role.Id))
                    {
                        data.LockedRoles.Remove(role.Id);
                    }

                    embedBuilder = InsanityBot.Embeds["insanitybot.admin.lock.unblacklist"]
                        .WithDescription(GetFormattedString(
                            InsanityBot.LanguageConfig["insanitybot.commands.lock.blacklist.remove.success"], role, channel));

                    channel.SerializeLockingOverwrites(data);

                    InsanityBot.Client.Logger.LogInformation(new EventId(2000, "LockAdmin"), $"Removed role {role.Id} from channel blacklist");

                }
                catch(Exception e)
                {
                    embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(
                            InsanityBot.LanguageConfig["insanitybot.commands.lock.blacklist.remove.failure"], role, channel));

                    InsanityBot.Client.Logger.LogCritical(new EventId(2000, "LockAdmin"),
                        $"Administrative action failed: could not remove {role.Id} from channel blacklist. Please contact the InsanityBot team immediately.\n" +
                        $"Please also provide them with the following information:\n\n{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                }
            }
        }
    }
}
