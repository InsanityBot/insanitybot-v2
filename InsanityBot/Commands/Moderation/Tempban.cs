using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Modlogs.SafeAccessInterface;
using InsanityBot.Utility.Permissions;
using InsanityBot.Utility.Timers;

using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation
{
    public partial class Ban
    {
        [Command("tempban")]
        [Aliases("temp-ban")]
        public async Task TempbanCommand(CommandContext ctx,
            DiscordMember member,
            String time,

            [RemainingText]
            String Reason = "usedefault")
        {
            if (time.StartsWith('-'))
            {
                await ParseTempbanCommand(ctx, member, String.Join(' ', time, Reason));
                return;
            }
            await ExecuteTempbanCommand(ctx, member,
                                time.ParseTimeSpan(TemporaryPunishmentType.Ban),
                                Reason, false, false);
        }

        private async Task ParseTempbanCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                {
                    cmdArguments += " --reason usedefault";
                }

                await Parser.Default.ParseArguments<TempbanOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteTempbanCommand(ctx, member,
                                o.Time.ParseTimeSpan(TemporaryPunishmentType.Ban),
                                String.Join(' ', o.Reason), o.Silent, o.DmMember);
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.ban.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel.SendMessageAsync(embed: failed.Build());
            }
        }

        private async Task ExecuteTempbanCommand(CommandContext ctx,
            DiscordMember member,
            TimeSpan time,
            String Reason,
            Boolean Silent,
            Boolean DmMember)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.tempban"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            String BanReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new()
            {
                Title = "TEMPBAN",
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Duration", time.ToString(), true)
                .AddField("Reason", BanReason, true);

            try
            {
                BanStartingEvent();

                Timer callbackTimer = new(DateTime.Now.Add(time), $"tempban_{member.Id}");
                moderationEmbedBuilder.AddField("Timer GUID", callbackTimer.Guid.ToString(), true);
                TimeHandler.AddTimer(callbackTimer);

                _ = member.TryAddModlogEntry(ModlogEntryType.ban, BanReason);
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.ban.success"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                _ = InsanityBot.HomeGuild.BanMemberAsync(member, 0, BanReason);
                _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Moderation, new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder
                });

            }
            catch
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.ban.failure"], ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
            }
            finally
            {
                if (embedBuilder == null)
                {
                    InsanityBot.Client.Logger.LogError(new EventId(1151, "Tempban"),
                        "Could not execute tempban command, an unknown exception occured.");
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }


        public static void InitializeUnban(String Identifier, Guid guid)
        {
            if (!Identifier.StartsWith("tempban_"))
            {
                return;
            }

            try
            {
                File.Delete($"./cache/timers/{Identifier}");

                new Ban().ExecuteUnbanCommand(null, ToUInt64(Identifier),
                    true, false, true, "timer_guid", guid).GetAwaiter().GetResult();

                UnbanCompletedEvent();
            }
            catch (Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1152, "Unban"), $"Could not unban user {Identifier[9..]}");
                System.Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
            }
        }

        private static DiscordMember GetMember(String Identifier)
        {
            Task<DiscordMember> thing = InsanityBot.HomeGuild.GetMemberAsync(ToUInt64(Identifier[9..]));
            return thing.GetAwaiter().GetResult();
        }

        public static event TimedActionCompleteEventHandler UnbanCompletedEvent;
        public static event TimedActionStartEventHandler BanStartingEvent;
    }

    public class TempbanOptions : ModerationOptionBase
    {
        [Option('t', "time", Default = "default", Required = false)]
        public String Time { get; set; }
    }
}
