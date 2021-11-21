namespace InsanityBot.Tickets.Commands.Admin.Presets;
using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using global::InsanityBot.Core.Attributes;

using Newtonsoft.Json;

public partial class PresetCommands
{
	[Group("category")]
	public class Category : BaseCommandModule
	{
		[GroupCommand]
		[RequireAdminPermission("insanitybot.tickets.presets.view")]
		public async Task GetCategory(CommandContext ctx, String presetId)
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
				.WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.category.get"]
					.Replace("{PRESET}", presetId)
					.Replace("{CATEGORY}", $"<#{preset.Category}>"));
			await ctx.Channel.SendMessageAsync(response.Build());
		}

		[Command("set")]
		[RequireAdminPermission("insanitybot.tickets.presets.edit")]
		public async Task SetCategory(CommandContext ctx, String presetId, UInt64 id)
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

				preset = preset with { Category = id };

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
				.WithDescription(InsanityBot.LanguageConfig["insanitybot.tickets.presets.category.set"]
					.Replace("{PRESET}", presetId)
					.Replace("{CATEGORY}", $"<#{preset.Category}>"));

			await ctx.Channel.SendMessageAsync(response.Build());

			DiscordEmbedBuilder ticketLog = InsanityBot.Embeds["insanitybot.ticketlog.presets.edit"]
				.AddField("Preset", presetId, true)
				.AddField("Category", $"<#{preset.Category}>", true);
			await InsanityBot.MessageLogger.LogMessage(ticketLog.Build(), ctx);
		}
	}
}