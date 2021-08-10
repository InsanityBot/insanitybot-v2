using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using InsanityBot.Commands.Moderation.Modlog.Individual;
using InsanityBot.Utility.Modlogs.Reference;
using InsanityBot.Utility.Modlogs.SafeAccessInterface;
using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public partial class Modlog : BaseCommandModule
    {
        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx,
            DiscordUser user)
        {
            if(!ctx.Member.HasPermission("insanitybot.moderation.modlog"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            try
            {
                _ = user.TryFetchModlog(out UserModlog modlog);

                DiscordEmbedBuilder modlogEmbed = null;

                if(modlog.ModlogEntryCount == 0)
                {
                    modlogEmbed = InsanityBot.Embeds["insanitybot.modlog.empty"];
                    modlogEmbed.Description = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.empty_modlog"],
                        ctx, user);
                    _ = ctx.Channel?.SendMessageAsync(embed: modlogEmbed.Build());
                }
                else
                {
                    modlogEmbed = InsanityBot.Embeds["insanitybot.modlog.entries"];

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

                    if(warns.Any())
                    {
                        modlogEmbed.AddField("Warns", warns.Count().ToString(), true);
                    }

                    if(mutes.Any())
                    {
                        modlogEmbed.AddField("Mutes", mutes.Count().ToString(), true);
                    }

                    if(blacklists.Any())
                    {
                        modlogEmbed.AddField("Blacklists", blacklists.Count().ToString(), true);
                    }

                    if(kicks.Any())
                    {
                        modlogEmbed.AddField("Kicks", kicks.Count().ToString(), true);
                    }

                    if(bans.Any())
                    {
                        modlogEmbed.AddField("Bans", bans.Count().ToString(), true);
                    }

                    if(!InsanityBot.Config.Value<Boolean>("insanitybot.commands.modlog.allow_scrolling"))
                    {
                        modlogEmbed.Description = user.CreateModlogDescription(false);

                        await ctx.Channel?.SendMessageAsync(embed: modlogEmbed.Build());
                    }
                    else
                    {
                        String embedDescription = user.CreateModlogDescription();

                        IEnumerable<DSharpPlus.Interactivity.Page> pages = InsanityBot.Interactivity.GeneratePagesInEmbed(embedDescription, SplitType.Line, modlogEmbed);

                        await ctx.Channel?.SendPaginatedMessageAsync(ctx.Member, pages);
                    }
                }
            }
            catch(Exception e)
            {
                InsanityBot.Client.Logger.LogError(new EventId(1170, "Modlog"), $"Could not retrieve modlogs: {e}: {e.Message}");

                DiscordEmbedBuilder failedModlog = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.failed"], ctx, user));
                
                await ctx.Channel?.SendMessageAsync(embed: failedModlog.Build());
            }
        }

        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx, String type, DiscordUser member)
        {
            switch(type.ToLower())
            {
                case "warn":
                case "warns":
                case "warnings":
                    await new WarnModlog().WarnModlogCommand(ctx, member);
                    break;
                case "mute":
                case "mutes":
                    await new MuteModlog().MuteModlogCommand(ctx, member);
                    break;
                case "blacklist":
                case "blacklists":
                    await new BlacklistModlog().BlacklistModlogCommand(ctx, member);
                    break;
                case "kick":
                case "kicks":
                    await new KickModlog().KickModlogCommand(ctx, member);
                    break;
                case "ban":
                case "bans":
                    await new BanModlog().BanModlogCommand(ctx, member);
                    break;
            }
        }

        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx, String type)
            => await this.ModlogCommand(ctx, type, ctx.Member);


        [Command("modlog")]
        public async Task ModlogCommand(CommandContext ctx)
            => await this.ModlogCommand(ctx, ctx.Member);
    }
}
