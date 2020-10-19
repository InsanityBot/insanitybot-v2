using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public class Mute : BaseCommandModule
    {
        [Command("mute")]
        [Description("Mutes the tagged user.")]
        public async Task MuteCommand(CommandContext ctx,

            [Description("Mention the user you want to mute")]
            DiscordMember member,
            
            [Description("Give a reason for the mute")]
            [RemainingText]
            String Reason = "usedefault")
        {
            if (!(await InsanityBot.PermissionManager.GetCacheEntry(ctx.Member.Id))["insanitybot.moderation.mute"])
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //actually do something with the usedefault value
            String MuteReason = Reason;
            if (Reason == "usedefault")
                MuteReason = InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"];

            MuteReason = GetFormattedString(MuteReason, ctx, member);
            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = "MUTE",
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", Reason, true);

            try
            {
                member.AddModlogEntry(ModlogEntryType.mute, Reason);
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.moderation.success"],
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
                _ = member.GrantRoleAsync(InsanityBot.HomeGuild.GetRole(
                    (UInt64)InsanityBot.Config["insanitybot.identifiers.moderation.mute_role_id"]),
                    MuteReason);
                _ = InsanityBot.HomeGuild.GetChannel((UInt64)InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"])
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
            catch
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.moderation.failure"],
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
            }
            finally
            {
                 await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }
    }
}
