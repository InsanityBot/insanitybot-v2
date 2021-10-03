using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Core.Attributes;

using Newtonsoft.Json;

namespace InsanityBot.Tickets.Commands.Admin.Presets
{
    public partial class PresetCommands
    {
        [Group("nameformat")]
        [Aliases("name-format")]
        public class PresetNameFormat
        {
            [GroupCommand]
            [RequireAdminPermission("insanitybot.tickets.presets.view")]
            public async Task GetNameFormat(CommandContext ctx, String presetId)
            {
                if(!File.Exists($"./cache/tickets/presets/{presetId}.json"))
                {
                    DiscordEmbedBuilder embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.error.generic"]);
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                    return;
                }

                TicketPreset preset;

                try
                {
                    StreamReader reader = new($"./cache/tickets/presets/{presetId}.json");
                    preset = JsonConvert.DeserializeObject<TicketPreset>(reader.ReadToEnd());
                    reader.Close();
                }
                catch
                {
                    DiscordEmbedBuilder embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.error.generic"]);
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                    return;
                }

                DiscordEmbedBuilder response = InsanityBot.Embeds["insanitybot.tickets.presets.view"]
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.name.get"]
                        .Replace("{PRESET}", presetId)
                        .Replace("{FORMAT}", preset.NameFormat));
                await ctx.Channel.SendMessageAsync(response.Build());
            }

            [Command("set")]
            [RequireAdminPermission("insanitybot.tickets.presets.edit")]
            public async Task SetCategory(CommandContext ctx, String presetId, String format)
            {
                if(!File.Exists($"./cache/tickets/presets/{presetId}.json"))
                {
                    DiscordEmbedBuilder embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.error.generic"]);
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                    return;
                }

                TicketPreset preset;

                try
                {
                    StreamReader reader = new($"./cache/tickets/presets/{presetId}.json");
                    preset = JsonConvert.DeserializeObject<TicketPreset>(reader.ReadToEnd());
                    reader.Close();

                    preset = preset with { NameFormat = format };

                    StreamWriter writer = new($"./cache/tickets/presets/{presetId}.json");
                    writer.Write(JsonConvert.SerializeObject(preset, Formatting.Indented));
                    writer.Close();
                }
                catch
                {
                    DiscordEmbedBuilder embedBuilder = InsanityBot.Embeds["insanitybot.error"]
                        .WithDescription(InsanityBot.LanguageConfig["insanitybot.error.generic"]);
                    await ctx.Channel.SendMessageAsync(embedBuilder.Build());
                    return;
                }

                DiscordEmbedBuilder response = InsanityBot.Embeds["insanitybot.tickets.presets.edit"]
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.name.set"]
                        .Replace("{PRESET}", presetId)
                        .Replace("{FORMAT}", format));

                await ctx.Channel.SendMessageAsync(response.Build());

                DiscordEmbedBuilder ticketLog = InsanityBot.Embeds["insanitybot.ticketlog.presets.edit"]
                    .AddField("Preset", presetId, true)
                    .AddField("Format", format, true);
                await InsanityBot.MessageLogger.LogMessage(ticketLog.Build(), ctx);
            }
        }
    }
}
