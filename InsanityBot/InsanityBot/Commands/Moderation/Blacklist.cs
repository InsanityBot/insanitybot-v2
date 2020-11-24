using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation
{
    public class Blacklist : BaseCommandModule
    {
        [Command("blacklist")]
        public async Task BlacklistCommand(CommandContext ctx,
            DiscordMember member,

            [RemainingText]
            String Reason = "usedefault")
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.blacklist"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //actually do something with the usedefault value
            String BlacklistReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = "BLACKLIST",
                Color = DiscordColor.Black,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", BlacklistReason, true);

            try
            {
                member.AddModlogEntry(ModlogEntryType.blacklist, BlacklistReason);
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.blacklist.success"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
                _ = member.GrantRoleAsync(InsanityBot.HomeGuild.GetRole(
                    ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.blacklist_role_id"])),
                    BlacklistReason);
                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.blacklist.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }

        [Command("whitelist")]
        public async Task WhitelistCommand(CommandContext ctx,
            DiscordMember member,

            [RemainingText]
            String Reason = "usedefault")
        {

            if (!ctx.Member.HasPermission("insanitybot.moderation.whitelist"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //actually do something with the usedefault value
            String WhitelistReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = "WHITELIST",
                Color = DiscordColor.White,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", WhitelistReason, true);
            try
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.success"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };

                _ = member.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
                    ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.blacklist_role_id"])),
                    WhitelistReason);
                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch (Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.whitelist.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }
    }
}
