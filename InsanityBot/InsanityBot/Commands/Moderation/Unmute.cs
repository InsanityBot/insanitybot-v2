using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation
{
    public partial class Mute
    {
        [Command("unmute")]
        public async Task UnmuteCommand(CommandContext ctx,
            DiscordMember member,
            String arguments = null)
        {
            if (arguments == null)
            {
                await ExecuteUnmuteCommand(ctx, member, false, false);
                return;
            }

            if (arguments.StartsWith('-'))
            {
                await ParseUnmuteCommand(ctx, member, arguments);
                return;
            }

            InsanityBot.Client.Logger.LogWarning(new EventId(1133, "ArgumentParser"),
                "Unmute command was called with invalid arguments, running default arguments");
            await ExecuteUnmuteCommand(ctx, member, false, false);
        }

        private async Task ParseUnmuteCommand(CommandContext ctx,
            DiscordMember member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if (!arguments.Contains("-r") && !arguments.Contains("--reason"))
                    cmdArguments += " --reason void"; //we dont need the reason but its required by the protocol

                await Parser.Default.ParseArguments<UnmuteOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await ExecuteUnmuteCommand(ctx, member, o.Silent, o.DmMember);
                    });
            }
            catch (Exception e)
            {
                DiscordEmbedBuilder failed = new DiscordEmbedBuilder
                {
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unmute.failure"],
                        ctx, member),
                    Color = DiscordColor.Red,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"), $"{e}: {e.Message}");

                await ctx.RespondAsync(embed: failed.Build());
            }
        }

        /* ctx can be null if automated is true since ctx is only used for two purposes
         * its used to respond to the command execution, which does not happen when silent mode is enabled
         * (silent is enforced by auto mode)
         * and its used to verify permissions, but that check is never called when auto mode is enabled */
        private async Task ExecuteUnmuteCommand(CommandContext ctx,
            DiscordMember member,
            Boolean silent,
            Boolean dmMember,
            Boolean automated = false,
            params Object[] additionals)
        {

            if (!automated && !ctx.Member.HasPermission("insanitybot.moderation.unmute"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            if (ctx == null && silent == false)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"),
                    "Invalid command arguments - internal error. Please report this on https://github.com/InsanityNetwork/InsanityBot/issues" +
                    "\nInsanityBot/Commands/Moderation/Unmute.cs: argument \"silent\" cannot be false without given command context");
                return;
            }
            if (automated && !silent)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"),
                    "Invalid command arguments - internal error. Please report this on https://github.com/InsanityNetwork/InsanityBot/issues" +
                    "\nInsanityBot/Commands/Moderation/Unmute.cs: argument \"silent\" cannot be false for an automated unmute");
                return;
            }

            DiscordEmbedBuilder nonSilent = null;
            DiscordEmbedBuilder moderationEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = "UNMUTE",
                Color = DiscordColor.SpringGreen,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020-2021"
                }
            };

            if (automated)
                moderationEmbedBuilder.AddField("Moderator", "InsanityBot", true);
            else
                moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true);

            moderationEmbedBuilder.AddField("Member", member.Mention, true);

            try
            {
                if (silent)
                {
                    _ = member.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
                        ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.mute_role_id"])),
                        "Silent unmute");

                    if (additionals != null)
                    {
                        for (Byte b = 0; b < additionals.Length; b++)
                        {
                            if (additionals[b] is String str && str == "timer_guid")
                            {
                                moderationEmbedBuilder.AddField("Timer Guid", ((Guid)additionals[b + 1]).ToString(), true);
                            }
                        }
                    }
                }
                else
                {
                    nonSilent = new DiscordEmbedBuilder
                    {
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unmute.success"],
                               ctx, member),
                        Color = DiscordColor.Green,
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            Text = "InsanityBot - ExaInsanity 2020-2021"
                        }
                    };


                    _ = member.RevokeRoleAsync(InsanityBot.HomeGuild.GetRole(
                        ToUInt64(InsanityBot.Config["insanitybot.identifiers.moderation.mute_role_id"])),
                        "unmute");

                    if (additionals.Length >= 2)
                    {
                        for (Byte b = 0; b <= additionals.Length; b++)
                        {
                            if (additionals[b] is String str && str == "timer_guid")
                            {
                                moderationEmbedBuilder.AddField("Timer Guid", ((Guid)additionals[b + 1]).ToString(), true);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!silent)
                    nonSilent = new DiscordEmbedBuilder
                    {
                        Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unmute.failure"],
                            ctx, member),
                        Color = DiscordColor.Red,
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            Text = "InsanityBot - ExaInsanity 2020-2021"
                        }
                    };
                InsanityBot.Client.Logger.LogError(new EventId(1134, "Unmute"), $"{e}: {e.Message}");
            }
            finally
            {
                if (!silent)
                    _ = ctx.RespondAsync(embed: nonSilent.Build());
                await InsanityBot.HomeGuild.GetChannel(ToUInt64(InsanityBot.Config["insanitybot.identifiers.commands.modlog_channel_id"]))
                    .SendMessageAsync(embed: moderationEmbedBuilder.Build());
            }
        }
    }

    public class UnmuteOptions : ModerationOptionBase
    {

    }
}
