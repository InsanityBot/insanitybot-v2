using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Services.Internal.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Modlogs.SafeAccessInterface;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation
{
    public partial class Mute : BaseCommandModule
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
            if(Reason.StartsWith('-'))
            {
                await this.ParseMuteCommand(ctx, member, Reason);
                return;
            }
            await this.ExecuteMuteCommand(ctx, member, Reason, false, false);
        }

        private async Task ParseMuteCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if(!arguments.Contains("-r") && !arguments.Contains("--reason"))
                {
                    cmdArguments += " --reason usedefault";
                }

                await Parser.Default.ParseArguments<MuteOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        if(o.Time == "default")
                        {
                            await this.ExecuteMuteCommand(ctx, member, String.Join(' ', o.Reason), o.Silent, o.DmMember);
                        }
                        else
                        {
                            await this.ExecuteTempmuteCommand(ctx, member,
                                o.Time.ParseTimeSpan(TemporaryPunishmentType.Mute),
                                String.Join(' ', o.Reason), o.Silent, o.DmMember);
                        }
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = new()
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.mute.failure"],
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

        private async Task ExecuteMuteCommand(CommandContext ctx,
            DiscordMember member,
            String Reason,
            Boolean Silent,
            Boolean DmMember)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.mute"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //actually do something with the usedefault value
            String MuteReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = new()
            {
                Title = "MUTE",
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot 2020-2021"
                }
            };

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", MuteReason, true);

            try
            {
                _ = member.TryAddModlogEntry(ModlogEntryType.mute, MuteReason);
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.mute.success"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                _ = member.GrantRoleAsync(InsanityBot.HomeGuild.GetRole(
                    ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.mute_role_id"])),
                    MuteReason);
                _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Moderation, new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder
                });
            }
            catch(Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.mute.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                await ctx.Channel.SendMessageAsync(embed: embedBuilder.Build());
            }
        }
    }

    public class MuteOptions : TempmuteOptions
    {

    }
}
