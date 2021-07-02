using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Core.Formatters.Embeds;
using InsanityBot.Tickets.Placeholders;

using System;
using System.Threading.Tasks;

namespace InsanityBot.Tickets
{
    public class TicketCreator
    {
        public async Task CreateTicket(TicketPreset preset, CommandContext context, String topic)
        {
            DiscordChannel TicketCategory = InsanityBot.HomeGuild.GetChannel(preset.Category);

            DiscordChannel ticket = await InsanityBot.HomeGuild.CreateChannelAsync($"insanitybot-temp-{TicketDaemon.TicketCount}",
                ChannelType.Private, TicketCategory);

            ProtoTicket proto = new()
            {
                Creator = context.Member.Id,
                DiscordChannelId = ticket.Id,
                Settings = preset.Settings,
                TicketGuid = Guid.NewGuid()
            };

            Task<Guid> finalGuidTask = InsanityBot.TicketDaemon.UpgradeProtoTicket(proto);

            // ticket channel manipulations according to the preset

            foreach(var v in preset.AccessRules.AllowedUsers)
            {
                _ = ticket.AddOverwriteAsync(await InsanityBot.HomeGuild.GetMemberAsync(v),
                    allow: Permissions.AccessChannels);
            }

            foreach(var v in preset.AccessRules.AllowedRoles)
            {
                _ = ticket.AddOverwriteAsync(InsanityBot.HomeGuild.GetRole(v),
                    allow: Permissions.AccessChannels);
            }

            // by now the ticket needs to be upgraded

            DiscordTicket virtualTicket = InsanityBot.TicketDaemon.Tickets[await finalGuidTask];

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
        }
    }
}
