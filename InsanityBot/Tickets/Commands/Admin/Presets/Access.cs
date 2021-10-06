using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using InsanityBot.Core.Attributes;

using Newtonsoft.Json;

namespace InsanityBot.Tickets.Commands.Admin.Presets
{
    public partial class PresetCommands
    {
        [Group("access")]
        public class Access : BaseCommandModule
        {
            [GroupCommand]
            [RequireAdminPermission("insanitybot.tickets.presets.view")]
            public async Task ViewAccessCommand(CommandContext ctx, String presetId)
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
                    .WithTitle(InsanityBot.LanguageConfig["insanitybot.tickets.presets.access.get"]
                        .Replace("{PRESET}", presetId));

                StringBuilder desc = new();
                desc.Append(InsanityBot.LanguageConfig["insanitybot.tickets.presets.access.roles"]);
                desc.Append('\n');

                foreach(UInt64 v in preset.AccessRules.AllowedRoles)
                {
                    desc.Append($"<@&{v}>\n");
                }

                desc.Append(InsanityBot.LanguageConfig["insanitybot.tickets.presets.access.users"]);
                desc.Append('\n');

                foreach(UInt64 v in preset.AccessRules.AllowedUsers)
                {
                    desc.Append($"<@{v}>\n");
                }

                response.WithDescription(desc.ToString());

                await ctx.Channel.SendMessageAsync(response.Build());
            }

            [Command("add")]
            [RequireAdminPermission("insanitybot.tickets.presets.edit")]
            public async Task AddAccess(CommandContext ctx, String presetId, DiscordUser user)
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

                    TicketAccess newRule = preset.AccessRules;
                    newRule.AllowedUsers = newRule.AllowedUsers.Append(user.Id).ToArray();

                    preset = preset with { AccessRules = newRule };

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
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.access.setuser"]
                        .Replace("{PRESET}", presetId)
                        .Replace("{USER}", user.Mention));

                await ctx.Channel.SendMessageAsync(response.Build());

                DiscordEmbedBuilder ticketLog = InsanityBot.Embeds["insanitybot.ticketlog.presets.edit"]
                    .AddField("Preset", presetId, true)
                    .AddField("User", user.Mention, true);
                await InsanityBot.MessageLogger.LogMessage(ticketLog.Build(), ctx);
            }

            [Command("add")]
            [RequireAdminPermission("insanitybot.tickets.presets.edit")]
            public async Task AddAccess(CommandContext ctx, String presetId, DiscordRole role)
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

                    TicketAccess newRule = preset.AccessRules;
                    newRule.AllowedRoles = newRule.AllowedRoles.Append(role.Id).ToArray();

                    preset = preset with { AccessRules = newRule };

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
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.access.setrole"]
                        .Replace("{PRESET}", presetId)
                        .Replace("{ROLE}", role.Mention));

                await ctx.Channel.SendMessageAsync(response.Build());

                DiscordEmbedBuilder ticketLog = InsanityBot.Embeds["insanitybot.ticketlog.presets.edit"]
                    .AddField("Preset", presetId, true)
                    .AddField("Role", role.Mention, true);
                await InsanityBot.MessageLogger.LogMessage(ticketLog.Build(), ctx);
            }

            [Command("revoke")]
            [Aliases("remove")]
            [RequireAdminPermission("insanitybot.tickets.presets.edit")]
            public async Task RevokeAccess(CommandContext ctx, String presetId, DiscordUser user)
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

                    TicketAccess newRule = preset.AccessRules;
                    newRule.AllowedUsers = (from v in preset.AccessRules.AllowedUsers
                                            where v != user.Id
                                            select v).ToArray();

                    preset = preset with { AccessRules = newRule };

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
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.access.setuser"]
                        .Replace("{PRESET}", presetId)
                        .Replace("{USER}", user.Mention));

                await ctx.Channel.SendMessageAsync(response.Build());

                DiscordEmbedBuilder ticketLog = InsanityBot.Embeds["insanitybot.ticketlog.presets.edit"]
                    .AddField("Preset", presetId, true)
                    .AddField("User", user.Mention, true);
                await InsanityBot.MessageLogger.LogMessage(ticketLog.Build(), ctx);
            }

            [Command("revoke")]
            [RequireAdminPermission("insanitybot.tickets.presets.edit")]
            public async Task RevokeAccess(CommandContext ctx, String presetId, DiscordRole role)
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

                    TicketAccess newRule = preset.AccessRules;
                    newRule.AllowedRoles = (from v in preset.AccessRules.AllowedRoles
                                            where v != role.Id
                                            select v).ToArray();

                    preset = preset with { AccessRules = newRule };

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
                    .WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.access.setrole"]
                        .Replace("{PRESET}", presetId)
                        .Replace("{ROLE}", role.Mention));

                await ctx.Channel.SendMessageAsync(response.Build());

                DiscordEmbedBuilder ticketLog = InsanityBot.Embeds["insanitybot.ticketlog.presets.edit"]
                    .AddField("Preset", presetId, true)
                    .AddField("Role", role.Mention, true);
                await InsanityBot.MessageLogger.LogMessage(ticketLog.Build(), ctx);
            }
        }
    }
}
