using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public partial class Ban
    {
        [Command("unban")]
        public async Task UnbanCommand(CommandContext ctx,
            DiscordUser member,

            [RemainingText]
            String arguments = null)
        {
            if(arguments == null)
            {
                await this.ExecuteUnbanCommand(ctx, member, false, false);
                return;
            }

            if(arguments.StartsWith('-'))
            {
                await this.ParseUnbanCommand(ctx, member, arguments);
                return;
            }

            InsanityBot.Client.Logger.LogWarning(new EventId(1143, "ArgumentParser"),
                "Unban command was called with invalid arguments, running default arguments");
            await this.ExecuteUnbanCommand(ctx, member, false, false);
        }

        private async Task ParseUnbanCommand(CommandContext ctx,
            DiscordUser member,
            String arguments)
        {
            String cmdArguments = arguments;
            try
            {
                if(!arguments.Contains("-r") && !arguments.Contains("--reason"))
                {
                    cmdArguments += " --reason void"; //we dont need the reason but its required by the protocol
                }

                await Parser.Default.ParseArguments<UnbanOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await this.ExecuteUnbanCommand(ctx, member, o.Silent, o.DmMember);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unban.failure"], ctx, member));
                
                InsanityBot.Client.Logger.LogError(new EventId(1144, "Unban"), $"{e}: {e.Message}");

                await ctx.Channel?.SendMessageAsync(embed: failed.Build());
            }
        }

        /* ctx can be null if automated is true since ctx is only used for two purposes
         * its used to respond to the command execution, which does not happen when silent mode is enabled
         * (silent is enforced by auto mode)
         * and its used to verify permissions, but that check is never called when auto mode is enabled */
        private async Task ExecuteUnbanCommand(CommandContext ctx,
            DiscordUser member,
            Boolean silent,
            Boolean automated = false,
            params Object[] additionals)
        {

            if(!automated && !ctx.Member.HasPermission("insanitybot.moderation.unban"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            if(ctx == null && silent == false)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1144, "Unban"),
                    "Invalid command arguments - internal error. Please report this on https://github.com/InsanityBot/InsanityBot/issues" +
                    "\nInsanityBot/Commands/Moderation/Unban.cs: argument \"silent\" cannot be false without given command context");
                return;
            }
            if(automated && !silent)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1144, "Unban"),
                    "Invalid command arguments - internal error. Please report this on https://github.com/InsanityBot/InsanityBot/issues" +
                    "\nInsanityBot/Commands/Moderation/Unban.cs: argument \"silent\" cannot be false for an automated unban");
                return;
            }

            DiscordEmbedBuilder nonSilent = null;
            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.unban"];

            if(automated)
            {
                moderationEmbedBuilder.AddField("Moderator", "InsanityBot", true);
            }
            else
            {
                moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true);
            }

            moderationEmbedBuilder.AddField("Member", member.ToString(), true);

            try
            {
                if(silent)
                {
                    await InsanityBot.HomeGuild.UnbanMemberAsync(member);

                    if(additionals != null)
                    {
                        for(Byte b = 0; b < additionals.Length; b++)
                        {
                            if(additionals[b] is String str && str == "timer_guid")
                            {
                                moderationEmbedBuilder.AddField("Timer Guid", ((Guid)additionals[b + 1]).ToString(), true);
                            }
                        }
                    }
                }
                else
                {
                    nonSilent = InsanityBot.Embeds["insanitybot.moderation.unban"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unban.success"], ctx, member));

                    await InsanityBot.HomeGuild.UnbanMemberAsync(member);

                    if(additionals.Length >= 2)
                    {
                        for(Byte b = 0; b <= additionals.Length; b++)
                        {
                            if(additionals[b] is String str && str == "timer_guid")
                            {
                                moderationEmbedBuilder.AddField("Timer Guid", ((Guid)additionals[b + 1]).ToString(), true);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                if(!silent)
                {
                    nonSilent = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unban.failure"], ctx, member));
                }

                InsanityBot.Client.Logger.LogError(new EventId(1144, "Unban"), $"{e}: {e.Message}");
            }
            finally
            {
                if(!silent)
                {
                    if(nonSilent != null)
                    {
                        _ = ctx.Channel?.SendMessageAsync(embed: nonSilent.Build());
                    }
                    else
                    {
                        InsanityBot.Client.Logger.LogError(new EventId(1145, "Unban"), $"DiscordEmbedBuilder nonSilent was null");
                    }
                }

                _ = InsanityBot.MessageLogger.LogMessage( new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder
                }, ctx);
            }
        }
    }

    public class UnbanOptions : ModerationOptionBase
    {

    }
}
