using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using System;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Commands
{
    public class NewTicketCommand : BaseCommandModule
    {
        [Command("new")]
        [Aliases("ticket", "create-ticket")]
        public async Task NewTicket(CommandContext ctx,
            [RemainingText]
            String data = "default")
        {
            if(!ctx.Member.HasPermission("insanitybot.tickets.new"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            String topic = data;
            TicketPreset preset = InsanityBot.TicketDaemon.DefaultPreset;

            foreach(TicketPreset v in InsanityBot.TicketDaemon.Presets)
            {
                if(data.ToLower().StartsWith(v.Id.ToLower()))
                {
                    topic = data[(data.Length - v.Id.Length)..];
                    preset = v;

                    if(String.IsNullOrWhiteSpace(topic))
                    {
                        topic = preset.Topic;
                    }

                    break;
                }
            }

            Guid ticketGuid = InsanityBot.TicketDaemon.CreateTicket(preset, ctx, topic);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Description = InsanityBot.LanguageConfig["insanitybot.tickets.new"].ReplaceValues(ctx,
                    InsanityBot.HomeGuild.GetChannel(InsanityBot.TicketDaemon.Tickets[ticketGuid].DiscordChannelId)),
                Color = DiscordColor.SpringGreen
            };

            await ctx.Channel.SendMessageAsync(embedBuilder.Build());
        }
    }
}
