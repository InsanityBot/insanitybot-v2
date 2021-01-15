using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation
{
    public partial class Modlog
    {
        [Command("verballog")]
        public async Task VerbalLogCommand(CommandContext ctx, 
            DiscordMember user)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.verballog"))
            {
                await ctx.RespondAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }
            try
            {
                UserModlog modlog = user.GetUserModlog();

                DiscordEmbedBuilder modlogEmbed = new DiscordEmbedBuilder
                {
                    Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.embed_title"],
                        ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - Exa 2020-2021"
                    }
                };

                if (modlog.VerbalLog.Count == 0)
                {
                    modlogEmbed.Color = DiscordColor.SpringGreen;
                    modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.empty_modlog"],
                        ctx, user);
                }
                else
                {
                    modlogEmbed.Color = DiscordColor.Red;
                    for (Byte b = 0; b < ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"])
                        && b < modlog.VerbalLog.Count; b++)
                    {
                        modlogEmbed.Description += $"{modlog.VerbalLog[b].Time} - {modlog.VerbalLog[b].Reason}\n";
                    }

                    if (modlog.VerbalLog.Count > ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]))
                    {
                        modlogEmbed.Description += GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.overflow"],
                            ctx, user);
                    }
                }

                await ctx.RespondAsync(embed: modlogEmbed.Build());
            }
            catch (Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1171, "VerbalLog"), $"Could not retrieve verbal logs: {e}: {e.Message}");

                DiscordEmbedBuilder failedModlog = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.failed"], ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                await ctx.RespondAsync(embed: failedModlog.Build());
            }
        }

        [Command("verballog")]
        public async Task VerbalLogCommand(CommandContext ctx)
            => await VerbalLogCommand(ctx, ctx.Member);
    }
}
