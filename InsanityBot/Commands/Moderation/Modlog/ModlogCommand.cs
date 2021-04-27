using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using InsanityBot.Utility.Modlogs;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using static System.Convert;
using static InsanityBot.Commands.StringUtilities;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public partial class Modlog : BaseCommandModule
    {
        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx,
            DiscordMember user)
        {
            if (!ctx.Member.HasPermission("insanitybot.moderation.modlog"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            try
            {
                UserModlog modlog = user.GetUserModlog();

                DiscordEmbedBuilder modlogEmbed = new()
                {
                    Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                        ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };

                if (modlog.ModlogEntryCount == 0)
                {
                    modlogEmbed.Color = DiscordColor.SpringGreen;
                    modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.empty_modlog"],
                        ctx, user);
                    _ = ctx.Channel.SendMessageAsync(embed: modlogEmbed.Build());
                }
                else
                {
                    IEnumerable<ModlogEntry> warns = from v in modlog.Modlog
                                                     where v.Type == ModlogEntryType.warn
                                                     select v;

                    IEnumerable<ModlogEntry> mutes = from v in modlog.Modlog
                                                     where v.Type == ModlogEntryType.mute
                                                     select v;

                    IEnumerable<ModlogEntry> blacklists = from v in modlog.Modlog
                                                          where v.Type == ModlogEntryType.blacklist
                                                          select v;

                    IEnumerable<ModlogEntry> kicks = from v in modlog.Modlog
                                                     where v.Type == ModlogEntryType.kick
                                                     select v;

                    IEnumerable<ModlogEntry> bans = from v in modlog.Modlog
                                                    where v.Type == ModlogEntryType.ban
                                                    select v;

                    if (warns.Any())
                    {
                        modlogEmbed.AddField("Warns", warns.Count().ToString(), true);
                    }

                    if (mutes.Any())
                    {
                        modlogEmbed.AddField("Mutes", mutes.Count().ToString(), true);
                    }

                    if (blacklists.Any())
                    {
                        modlogEmbed.AddField("Blacklists", blacklists.Count().ToString(), true);
                    }

                    if (kicks.Any())
                    {
                        modlogEmbed.AddField("Kicks", kicks.Count().ToString(), true);
                    }

                    if (bans.Any())
                    {
                        modlogEmbed.AddField("Bans", bans.Count().ToString(), true);
                    }

                    if (!ToBoolean(InsanityBot.Config["insanitybot.commands.modlog.allow_scrolling"]))
                    {
                        modlogEmbed.Color = DiscordColor.Red;
                        modlogEmbed.Description = user.CreateModlogDescription(false);

                        await ctx.Channel.SendMessageAsync(embed: modlogEmbed.Build());
                    }
                    else
                    {
                        modlogEmbed.Color = DiscordColor.Red;
                        String embedDescription = user.CreateModlogDescription();

                        IEnumerable<DSharpPlus.Interactivity.Page> pages = InsanityBot.Interactivity.GeneratePagesInEmbed(embedDescription, SplitType.Line, modlogEmbed);

                        await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
                    }
                }
            }
            catch (Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1170, "Modlog"), $"Could not retrieve modlogs: {e}: {e.Message}");

                DiscordEmbedBuilder failedModlog = new()
                {
                    Color = DiscordColor.Red,
                    Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.failed"], ctx, user),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot 2020-2021"
                    }
                };
                await ctx.Channel.SendMessageAsync(embed: failedModlog.Build());
            }
        }

        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx)
            => await ModlogCommand(ctx, ctx.Member);
    }
}
