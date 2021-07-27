using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using InsanityBot.Utility.Permissions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityBot.Tickets.Commands
{
    public class NewTicketCommand : BaseCommandModule
    {
        [Command("new")]
        [Aliases("ticket", "create-ticket")]
        public async Task CreateTicketCommand(CommandContext ctx,
            String data)
        {
            if(!ctx.Member.HasPermission("insanitybot.tickets.new"))
            {
                await ctx.Channel.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_permission"]);
                return;
            }

            String topic = data;
            TicketPreset preset = InsanityBot.TicketDaemon.DefaultPreset;

            foreach(var v in InsanityBot.TicketDaemon.Presets)
            {
                if(data.ToLower().StartsWith(v.Id.ToLower()))
                {
                    topic = data[(data.Length - v.Id.Length)..];
                    preset = v;
                    break;
                }
            }

            await InsanityBot.TicketDaemon.CreateTicket(preset, ctx, topic);
        }
    }
}
