using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Timers;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public class Tempmute : BaseCommandModule
    {
        [Command("tempmute")]
        [Aliases("temp-mute")]
        [Description("Temporarily mutes an user.")]
        public async Task TempmuteCommand(CommandContext ctx,
            
            [Description("The user to mute")]
            DiscordMember member,
            
            [Description("Duration of the mute")]
            TimeSpan time,
            
            [Description("Reason of the mute")]
            [RemainingText]
            String Reason = "usedefault")
        {
            if (!(await InsanityBot.PermissionManager.GetCacheEntry(ctx.Member.Id))["insanitybot.moderation.tempmute"])
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            String MuteReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = "TEMPMUTE",
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Duration", time.ToString(), true)
                .AddField("Reason", MuteReason, true);

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

                TimeHandler.ActiveTimers.Add(new Timer(DateTime.UtcNow.Add(time), $"tempmute_{member.Id}"));
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
