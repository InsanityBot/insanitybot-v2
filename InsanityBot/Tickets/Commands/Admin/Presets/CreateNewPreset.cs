using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions;

using Newtonsoft.Json;

namespace InsanityBot.Tickets.Commands.Admin.Presets
{
    [Group("preset")]
    public partial class PresetCommands : BaseCommandModule
    {
        [Command("create")]
        [Aliases("new")]
        public async Task CreatePresetCommand(CommandContext ctx, String name)
        {
            if(!ctx.Member.HasPermission("insanitybot.tickets.presets.create"))
            {
                await ctx.Channel?.SendMessageAsync(InsanityBot.LanguageConfig["insanitybot.error.lacking_admin_permission"]);
                return;
            }

            TicketPreset preset = new()
            {
                Id = name
            };

            StreamWriter writer = new(File.Create($"./cache/tickets/presets/{name}.json"));

            writer.Write(JsonConvert.SerializeObject(preset, Formatting.Indented));
            writer.Close();

            DiscordEmbedBuilder response = InsanityBot.Embeds["insanitybot.tickets.preset.new"];
            DiscordEmbedBuilder log = InsanityBot.Embeds["insanitybot.ticketlog.preset.new"]
                .AddField("Admin", ctx.Member.Mention, true)
                .AddField("Name", name, true);

            await ctx.Channel?.SendMessageAsync(response.Build());
            await InsanityBot.MessageLogger.LogMessage(log.Build(), ctx);
        }
    }
}
