using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Core.Formatters.Embeds;
using InsanityBot.Tickets.Placeholders;

using System;
using System.Collections.Generic;

namespace InsanityBot.Tickets
{
    public class TicketCreator
    {
        public Guid CreateTicket(TicketPreset preset, CommandContext context, String topic, out DiscordChannel ticket)
        {
            DiscordChannel TicketCategory = InsanityBot.HomeGuild.GetChannel(preset.Category);

            List<DiscordOverwriteBuilder> permissions = new();

            permissions.Add(new DiscordOverwriteBuilder(InsanityBot.HomeGuild.EveryoneRole)
                .Deny(Permissions.AccessChannels));

            foreach(UInt64 v in preset.AccessRules.AllowedUsers)
            {
                permissions.Add(new DiscordOverwriteBuilder(InsanityBot.HomeGuild.GetMemberAsync(v).GetAwaiter().GetResult())
                    .Allow(Permissions.AccessChannels));
            }

            foreach(UInt64 v in preset.AccessRules.AllowedRoles)
            {
                permissions.Add(new DiscordOverwriteBuilder(InsanityBot.HomeGuild.GetRole(v))
                    .Allow(Permissions.AccessChannels));
            }

            permissions.Add(new DiscordOverwriteBuilder(InsanityBot.HomeGuild.GetMemberAsync(InsanityBot.Client.CurrentUser.Id).GetAwaiter().GetResult())
                .Allow(Permissions.AccessChannels));

            permissions.Add(new DiscordOverwriteBuilder(context.Member)
                .Allow(Permissions.AccessChannels));

            ticket = InsanityBot.HomeGuild.CreateChannelAsync($"insanitybot-temp-{InsanityBot.TicketDaemon.TicketCount}",
                ChannelType.Text, TicketCategory, overwrites: permissions).GetAwaiter().GetResult();

            ProtoTicket proto = new()
            {
                Creator = context.Member.Id,
                DiscordChannelId = ticket.Id,
                Settings = preset.Settings,
                TicketGuid = Guid.NewGuid()
            };

            Guid finalGuid = InsanityBot.TicketDaemon.UpgradeProtoTicket(proto);

            DiscordTicket virtualTicket = InsanityBot.TicketDaemon.Tickets[finalGuid];

            foreach(String v in preset.CreationMessages)
            {
                String s = Parser.ParseTicketPlaceholders(v, virtualTicket.TicketGuid).GetAwaiter().GetResult();
                _ = ticket?.SendMessageAsync(s);
            }

            foreach(String v in preset.CreationEmbeds)
            {
                DiscordEmbed e = ((EmbedFormatter)InsanityBot.EmbedFactory.GetFormatter()).Read(
                    Parser.ParseTicketPlaceholders(v, virtualTicket.TicketGuid).GetAwaiter().GetResult());
                _ = ticket?.SendMessageAsync(e);
            }

            ticket.ModifyAsync(async xm =>
            {
                xm.Name = await Parser.ParseTicketPlaceholders(preset.NameFormat, virtualTicket.TicketGuid);
                xm.Topic = await Parser.ParseTicketPlaceholders(topic ?? preset.Topic, virtualTicket.TicketGuid);
            });

            virtualTicket.Topic = ticket.Topic;

            return virtualTicket.TicketGuid;
        }
    }
}
