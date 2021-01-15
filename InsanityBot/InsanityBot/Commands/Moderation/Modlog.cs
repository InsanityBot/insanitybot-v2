using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;
using Microsoft.Extensions.Logging;

namespace InsanityBot.Commands.Moderation
{
    public partial class Modlog : BaseCommandModule
    {
        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx,
            DiscordMember user)
        {
            try
            {
                UserModlog modlog = user.GetUserModlog();

                DiscordEmbedBuilder modlogEmbed = new DiscordEmbedBuilder
                {
                    Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                        ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - Exa 2020-2021"
                    }
                };

                if(modlog.Modlog.Count == 0)
                {
                    modlogEmbed.Color = DiscordColor.SpringGreen;
                    modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.empty_modlog"],
                        ctx, user);
                }
                else
                {
                    modlogEmbed.Color = DiscordColor.Red;
                    for(Byte b = 0; b < ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"])
                        && b < modlog.Modlog.Count; b++)
                    {
                        modlogEmbed.Description += $"{modlog.Modlog[b].Type.ToString().ToUpper()}: {modlog.Modlog[b].Time} - {modlog.Modlog[b].Reason}\n";
                    }
                    
                    if(modlog.Modlog.Count > ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]))
                    {
                        modlogEmbed.Description += GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.overflow"],
                            ctx, user);
                    }
                }

                await ctx.RespondAsync(embed: modlogEmbed.Build());
            }
            catch(Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1170, "Modlog"), $"Could not retrieve modlogs: {e}: {e.Message}");

                DiscordEmbedBuilder failedModlog = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.failed"], ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020-2021"
                    }
                };
                await ctx.RespondAsync(embed: failedModlog.Build());
            }
        }

        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx)
            => await ModlogCommand(ctx, ctx.Member);
    }
}
