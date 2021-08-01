using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Commands
{
    public class AddUserCommand : BaseCommandModule
    {
        [Command("add")]
        public async Task AddUser(CommandContext ctx,
            params DiscordMember[] users)
        {
            IEnumerable<KeyValuePair<Guid, DiscordTicket>> x = from t in InsanityBot.TicketDaemon.Tickets
                                                               where t.Value.DiscordChannelId == ctx.Channel.Id
                                                               select t;

            if(!x.Any())
            {
                DiscordEmbedBuilder error = new()
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.tickets.add_user.not_a_ticket_channel"].ReplaceValues(
                        ctx, ctx.Channel),
                    Color = DiscordColor.Red
                };

                _ = ctx.Channel.SendMessageAsync(error.Build());
                return;
            }

            foreach(var v in users)
            {
                _ = ctx.Channel.AddOverwriteAsync(v, Permissions.AccessChannels);

                if((Boolean)TicketDaemon.Configuration["insanitybot.tickets.ghost_mention_added_members"])
                {
                    DiscordMessage msg = await ctx.Channel.SendMessageAsync(v.Mention);
                    _ = msg.DeleteAsync();
                }
            }
        }

        public async Task AddRole(CommandContext ctx,
            params DiscordRole[] roles)
        {
            IEnumerable<KeyValuePair<Guid, DiscordTicket>> x = from t in InsanityBot.TicketDaemon.Tickets
                                                               where t.Value.DiscordChannelId == ctx.Channel.Id
                                                               select t;

            if(!x.Any())
            {
                DiscordEmbedBuilder error = new()
                {
                    Description = InsanityBot.LanguageConfig["insanitybot.tickets.add_user.not_a_ticket_channel"].ReplaceValues(
                        ctx, ctx.Channel),
                    Color = DiscordColor.Red
                };

                _ = ctx.Channel.SendMessageAsync(error.Build());
                return;
            }

            foreach(var v in roles)
            {
                _ = ctx.Channel.AddOverwriteAsync(v, Permissions.AccessChannels);

                if((Boolean)TicketDaemon.Configuration["insanitybot.tickets.ghost_mention_added_members"])
                {
                    DiscordMessage msg = await ctx.Channel.SendMessageAsync(v.Mention);
                    _ = msg.DeleteAsync();
                }
            }
        }
    }
}
