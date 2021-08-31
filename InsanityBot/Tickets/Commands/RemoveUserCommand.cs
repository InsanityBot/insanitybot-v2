using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Attributes;
using InsanityBot.Utility.Permissions;

namespace InsanityBot.Tickets.Commands
{
    public class RemoveUserCommand : BaseCommandModule
    {
        [Command("remove")]
        [RequirePermission("insanitybot.tickets.remove")]
        public async Task RemoveUser(CommandContext ctx,
            params DiscordMember[] users)
        {
            IEnumerable<KeyValuePair<Guid, DiscordTicket>> x = from t in InsanityBot.TicketDaemon.Tickets
                                                               where t.Value.DiscordChannelId == ctx.Channel?.Id
                                                               select t;

            KeyValuePair<Guid, DiscordTicket> y = x.First();

            if(!x.Any())
            {
                DiscordEmbedBuilder error = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.remove_user.not_a_ticket_channel"]
                        .ReplaceValues(ctx, ctx.Channel));

                await ctx.Channel?.SendMessageAsync(error.Build());
                return;
            }

            foreach(DiscordMember v in users)
            {
                _ = ctx.Channel?.AddOverwriteAsync(v, deny: Permissions.AccessChannels);

                DiscordTicket z = InsanityBot.TicketDaemon.Tickets[y.Key];
                z.AddedUsers = (from a in z.AddedUsers
                                where a != v.Id
                                select a).ToList();
                InsanityBot.TicketDaemon.Tickets[y.Key] = z;
            }
        }

        [Command("remove")]
        public async Task RemoveRole(CommandContext ctx,
            params DiscordRole[] roles)
        {
            if(!ctx.Member.HasPermission("insanitybot.tickets.remove"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            IEnumerable<KeyValuePair<Guid, DiscordTicket>> x = from t in InsanityBot.TicketDaemon.Tickets
                                                               where t.Value.DiscordChannelId == ctx.Channel?.Id
                                                               select t;

            if(!x.Any())
            {
                DiscordEmbedBuilder error = new()
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.tickets.remove_user.not_a_ticket_channel"].ReplaceValues(
                        ctx, ctx.Channel),
                    Color = DiscordColor.Red
                };

                await ctx.Channel?.SendMessageAsync(error.Build());
                return;
            }

            foreach(DiscordRole v in roles)
            {
                _ = ctx.Channel?.AddOverwriteAsync(v, deny: Permissions.AccessChannels);
            }
        }
    }
}
