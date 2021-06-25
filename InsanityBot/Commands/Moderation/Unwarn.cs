using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation
{
    public partial class Warn
    {
        // WarningIndex is zero-based, guys. id highly recommend using the string command instead
        [Command("unwarn")]
        public async Task UnwarnCommand(CommandContext ctx,
            DiscordMember member,
            Int32 WarningIndex)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.unwarn"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            DiscordEmbedBuilder embedBuilder = null;

            try
            {
                UserModlog modlog = member.GetUserModlog();
                modlog.Modlog.RemoveAt(WarningIndex);
                modlog.ModlogEntryCount--;
                member.SetUserModlog(modlog);

                embedBuilder = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.SpringGreen,
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unwarn.success"],
                        ctx, member),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
            }
            catch(Exception e)
            {
                embedBuilder = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.moderation.unwarn.failure"],
                        ctx, member),
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

        [Command("unwarn")]
        public async Task UnwarnCommand(CommandContext ctx,
            DiscordMember member,
            [RemainingText]
            String WarningText)
        {
            await this.UnwarnCommand(ctx, member,
                member.GetUserModlog().Modlog.IndexOf(member.GetUserModlog().Modlog.FirstOrDefault(md =>
                {
                    return md.Reason.Contains(WarningText);
                })));
        }
    }
}
