using CommandLine;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Modlogs.SafeAccessInterface;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    internal class Kick : BaseCommandModule
    {
        [Command("kick")]
        public async Task KickCommand(CommandContext ctx,
            DiscordMember member,

            [RemainingText]
            String arguments = "usedefault")
        {
            if(arguments.StartsWith('-'))
            {
                await this.ParseKickCommand(ctx, member, arguments);
                return;
            }
            await this.ExecuteKickCommand(ctx, member, arguments, false, false, false);
        }

        private async Task ParseKickCommand(CommandContext ctx,
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

                await Parser.Default.ParseArguments<KickOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        await this.ExecuteKickCommand(ctx, member, String.Join(' ', o.Reason), o.Silent, o.DmMember, o.SendInvite);
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.kick.failure"], ctx, member));

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel?.SendMessageAsync(embed: failed.Build());
            }
        }

        private async Task ExecuteKickCommand(CommandContext ctx,
            DiscordMember member,
            String Reason,
            Boolean Silent,
            Boolean DmMember,
            Boolean Invite)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.kick"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //actually do something with the usedefault value
            String KickReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.kick"];

            moderationEmbedBuilder.AddField("Moderator", ctx.Member?.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", KickReason, true);

            try
            {
                _ = member.TryAddModlogEntry(ModlogEntryType.kick, KickReason);
                embedBuilder = InsanityBot.Embeds["insanitybot.moderation.kick"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.kick.success"], ctx, member));

                if(Invite || DmMember)
                {
                    DiscordDmChannel channel = await member.CreateDmChannelAsync();
                    if(DmMember)
                    {
                        await channel?.SendMessageAsync(GetReason(GetFormattedString(
                            InsanityBot.LanguageConfig["insanitybot.moderation.kick.reason"],
                            ctx, member), KickReason));
                    }

                    if(Invite)
                    {
                        await channel?.SendMessageAsync((await ctx.Channel?.CreateInviteAsync()).ToString());
                    }
                }

                _ = member.RemoveAsync(KickReason);
                _ = InsanityBot.MessageLogger.LogMessage( new DiscordMessageBuilder
                {
                    Embed = moderationEmbedBuilder
                }, ctx);
            }
            catch(Exception e)
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.kick.failure"],
                        ctx, member));

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                if(!Silent)
                {
                    await ctx.Channel?.SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }
    }

    public class KickOptions : ModerationOptionBase
    {
        [Option('i', "invite", Default = false, Required = false)]
        public Boolean SendInvite { get; set; }
    }
}
