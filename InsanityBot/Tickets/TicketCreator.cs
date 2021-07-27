using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Core.Formatters.Embeds;
using InsanityBot.Tickets.Placeholders;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsanityBot.Tickets
{
    public class TicketCreator
    {
        public async Task<Guid> CreateTicket(TicketPreset preset, CommandContext context, String topic)
        {
            DiscordChannel TicketCategory = InsanityBot.HomeGuild.GetChannel(preset.Category);

            List<DiscordOverwriteBuilder> permissions = new();

            permissions.Add(new DiscordOverwriteBuilder()
                .Deny(Permissions.AccessChannels)
                .For(InsanityBot.HomeGuild.EveryoneRole));

            foreach(var v in preset.AccessRules.AllowedUsers)
            {
                permissions.Add(new DiscordOverwriteBuilder()
                    .Allow(Permissions.AccessChannels)
                    .For(await InsanityBot.HomeGuild.GetMemberAsync(v)));
            }

            foreach(var v in preset.AccessRules.AllowedRoles)
            {
                permissions.Add(new DiscordOverwriteBuilder()
                    .Allow(Permissions.AccessChannels)
                    .For(InsanityBot.HomeGuild.GetRole(v)));
            }

            DiscordChannel ticket = await InsanityBot.HomeGuild.CreateChannelAsync($"insanitybot-temp-{TicketDaemon.TicketCount}",
                ChannelType.Text, TicketCategory, overwrites: permissions);

            ProtoTicket proto = new()
            {
                Creator = context.Member.Id,
                DiscordChannelId = ticket.Id,
                Settings = preset.Settings,
                TicketGuid = Guid.NewGuid()
            };

            Guid finalGuid = await InsanityBot.TicketDaemon.UpgradeProtoTicket(proto);

            DiscordTicket virtualTicket = InsanityBot.TicketDaemon.Tickets[finalGuid];

            foreach(var v in preset.CreationMessages)
            {
                String s = await Parser.ParseTicketPlaceholders(v, virtualTicket.TicketGuid);
                _ = ticket.SendMessageAsync(s);
            }

            foreach(var v in preset.CreationEmbeds)
            {
                DiscordEmbed e = ((EmbedFormatter)InsanityBot.EmbedFactory.GetFormatter()).Read(
                    await Parser.ParseTicketPlaceholders(v, virtualTicket.TicketGuid));
                _ = ticket.SendMessageAsync(e);
            }

            await ticket.ModifyAsync(async xm =>
            {
                xm.Name = await Parser.ParseTicketPlaceholders(preset.NameFormat, virtualTicket.TicketGuid);
                xm.Topic = await Parser.ParseTicketPlaceholders(topic ?? preset.Topic, virtualTicket.TicketGuid);
            });

            virtualTicket.Topic = ticket.Topic;

            return virtualTicket.TicketGuid;
        }
    }
}
