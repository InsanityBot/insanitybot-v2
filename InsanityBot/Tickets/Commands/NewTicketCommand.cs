using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Microsoft.Extensions.Logging;

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
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
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

            Guid ticketGuid = InsanityBot.TicketDaemon.CreateTicket(preset, ctx, topic, out DiscordChannel channel);

            DiscordEmbedBuilder embedBuilder;
            DiscordEmbedBuilder logBuilder = InsanityBot.Embeds["insanitybot.ticketlog.new"]
                .AddField("Creator", ctx.Member.Mention, true)
                .AddField("Ticket", channel.Mention, true);

            await InsanityBot.MessageLogger.LogMessage(logBuilder.Build(), ctx);

            try
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.tickets.new"]
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.new"].ReplaceValues(ctx, channel));
            }
            catch(Exception e)
            {
                embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                    .WithDescription("Failed to create ticket");

                InsanityBot.Client.Logger.LogError($"{e}: {e.Message}\n{e.StackTrace}");
            }

            await ctx.Channel?.SendMessageAsync(embedBuilder.Build());
        }
    }
}
