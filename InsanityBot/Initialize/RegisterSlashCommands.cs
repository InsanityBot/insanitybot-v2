namespace InsanityBot;
using System;

using global::InsanityBot.SlashCommands.Moderation;

public partial class InsanityBot
{
	private static void RegisterSlashCommands()
	{
		if(!Config.Value<Boolean>("insanitybot.modules.slashcommands"))
		{
			return;
		}

		if(Config.Value<Boolean>("insanitybot.modules.moderation"))
		{
			SlashCommandsExtension.RegisterCommands<VerbalWarnSlashCommand>(Config.GuildId);
			SlashCommandsExtension.RegisterCommands<WarnSlashCommand>(Config.GuildId);
			SlashCommandsExtension.RegisterCommands<MuteSlashCommand>(Config.GuildId);
			SlashCommandsExtension.RegisterCommands<BlacklistSlashCommand>(Config.GuildId);
			SlashCommandsExtension.RegisterCommands<KickSlashCommand>(Config.GuildId);
			SlashCommandsExtension.RegisterCommands<BanSlashCommand>(Config.GuildId);

			SlashCommandsExtension.RegisterCommands<PurgeSlashCommand>(Config.GuildId);
		}
	}
}