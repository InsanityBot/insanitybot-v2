using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;
using InsanityBot.Utility.Timers;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;
using System.IO;

namespace InsanityBot.Commands.Moderation
{
    public partial class Mute : BaseCommandModule
    {
        [Command("tempmute")]
        [Aliases("temp-mute")]
        [Description("Temporarily mutes an user.")]
        public async Task TempmuteCommand(CommandContext ctx,
            
            [Description("The user to mute")]
            DiscordMember member,
            
            [Description("Duration of the mute")]
            String time,
            
            [Description("Reason of the mute")]
            [RemainingText]
            String Reason = "usedefault")
        {
            if(time.StartsWith('-'))
            {
                await ParseTempmuteCommand(ctx, member, String.Join(' ', time, Reason));
                return;
            }
            await ExecuteTempmuteCommand(ctx, member,
                                time.ParseTimeSpan(TemporaryPunishmentType.Mute),
                                Reason, false, false);
        }

        private async Task ParseTempmuteCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                    cmdArguments += " --reason usedefault";

                await Parser.Default.ParseArguments<TempmuteOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteTempmuteCommand(ctx, member,
                                o.Time.ParseTimeSpan(TemporaryPunishmentType.Mute),
                                String.Join(' ', o.Reason), o.Silent, o.DmMember);
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.warn.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.RespondAsync(embed: failed.Build());
            }
        }

        private async Task ExecuteTempmuteCommand(CommandContext ctx,
            DiscordMember member,
            TimeSpan time,
            String Reason,
            Boolean Silent,
            Boolean DmMember)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.tempmute"))
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
                Timer callbackTimer = new Timer(DateTime.Now.Add(time), $"tempmute_{member.Id}");
                moderationEmbedBuilder.AddField("Timer GUID", callbackTimer.Guid.ToString(), true);
                TimeHandler.AddTimer(callbackTimer);

                member.AddModlogEntry(ModlogEntryType.mute, MuteReason);
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.mute.success"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
                _ = member.GrantRoleAsync(InsanityBot.HomeGuild.GetRole(
                    ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.mute_role_id"])),
                    MuteReason);
                _ = InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());

            }
            catch
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.mute.failure"], ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    }
                };
            }
            finally
            {
                if(embedBuilder == null)
                {
                    InsanityBot.Client.Logger.LogError(new EventId(1041, "Tempmute"),
                        "Could not execute tempmute command, an unknown exception occured.");
                }
                await ctx.RespondAsync(embed: embedBuilder.Build());
            }
        }

        public static async Task InitializeUnmute(String Identifier, Guid guid)
        {
            if (!Identifier.StartsWith("tempmute_"))
                return;

            DiscordMember toUnmute = null;

            try
            {
                toUnmute = await InsanityBot.HomeGuild.GetMemberAsync(ToUInt64(Identifier[9..]));
                await toUnmute.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
                    ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.mute_role_id"])));

                File.Delete($"./data/timers/{Identifier}");
            }
            catch(Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1132, "Unmute"), $"Could not unmute user {toUnmute.Id}");
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    public class TempmuteOptions : ModerationOptionBase
    {
        [Option('t', "time", Default = "default", Required = false)]
        public String Time { get; set; }
    }
}
