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

namespace InsanityBot.Commands.Moderation
{
    public partial class Ban : BaseCommandModule
    {
        [Command("ban")]
        public async Task BanCommand(CommandContext ctx,
            DiscordMember member,

            [RemainingText]
            String Reason = "usedefault")
        {
            if(Reason.StartsWith('-'))
            {
                await this.ParseBanCommand(ctx, member, Reason);
                return;
            }
            await this.ExecuteBanCommand(ctx, member, Reason, false, false);
        }

        private async Task ParseBanCommand(CommandContext ctx,
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

                await Parser.Default.ParseArguments<BanOptions>(cmdArguments.Split(' '))
                    .WithParsedAsync(async o =>
                    {
                        if(o.Time == "default")
                        {
                            await this.ExecuteBanCommand(ctx, member, String.Join(' ', o.Reason), o.Silent, o.DmMember);
                        }
                        else
                        {
                            await this.ExecuteTempbanCommand(ctx, member,
                                o.Time.ParseTimeSpan(TemporaryPunishmentType.Ban),
                                String.Join(' ', o.Reason), o.Silent, o.DmMember);
                        }
                    });
            }
            catch(Exception e)
            {
                DiscordEmbedBuilder failed = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.ban.failure"], ctx, member));
                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");

                await ctx.Channel.SendMessageAsync(embed: failed.Build());
            }
        }

        private async Task ExecuteBanCommand(CommandContext ctx,
            DiscordMember member,
            String Reason,
            Boolean Silent,
            Boolean DmMember)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.ban"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            //actually do something with the usedefault value
            String BanReason = Reason switch
            {
                "usedefault" => GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.no_reason_given"],
                                ctx, member),
                _ => GetFormattedString(Reason, ctx, member)
            };

            DiscordEmbedBuilder embedBuilder = null;

            DiscordEmbedBuilder moderationEmbedBuilder = InsanityBot.Embeds["insanitybot.modlog.ban"];

            moderationEmbedBuilder.AddField("Moderator", ctx.Member.Mention, true)
                .AddField("Member", member.Mention, true)
                .AddField("Reason", BanReason, true);

            try
            {
                _ = member.TryAddModlogEntry(ModlogEntryType.ban, BanReason);
                embedBuilder = InsanityBot.Embeds["insanitybot.moderation.ban"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.ban.success"], ctx, member));

                _ = InsanityBot.HomeGuild.BanMemberAsync(member, 0, BanReason);
                _ = InsanityBot.ModlogQueue.QueueMessage(ModlogMessageType.Moderation, new DiscordMessageBuilder()
                {
                    Embed = moderationEmbedBuilder.Build()
                });
            }
            catch(Exception e)
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.error"].
                    WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.ban.failure"], ctx, member));

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}");
            }
            finally
            {
                await ctx.Channel.SendMessageAsync(embed: embedBuilder.Build());
            }
        }
    }

    public class BanOptions : TempbanOptions
    {

    }
}
