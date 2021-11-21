namespace InsanityBot.SlashCommands;
using System;
using System.Runtime.CompilerServices;

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

public static class StringUtilities
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context, DiscordMember commandTarget) => value.Replace("{MENTION}", commandTarget.Mention)
			.Replace("{USERNAME}", commandTarget.Username)
			.Replace("{NICKNAME}", commandTarget.Nickname)
			.Replace("{ID}", commandTarget.Id.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context, DiscordUser commandTarget) => value.Replace("{MENTION}", commandTarget.Mention)
			.Replace("{USERNAME}", commandTarget.Username)
			.Replace("{ID}", commandTarget.Id.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context, DiscordRole commandTarget) => value.Replace("{MENTION}", commandTarget.Mention)
			.Replace("{NAME}", commandTarget.Name)
			.Replace("{ID}", commandTarget.Id.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context, DiscordMember commandTarget, DiscordRole role)
		=> value.Replace("{MENTION}", commandTarget.Mention)
			.Replace("{USERNAME}", commandTarget.Username)
			.Replace("{NICKNAME}", commandTarget.Nickname)
			.Replace("{ID}", commandTarget.Id.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name)
			.Replace("{ROLE}", role.Mention)
			.Replace("{ROLENAME}", role.Name)
			.Replace("{ROLEID}", role.Id.ToString());


	public static String GetFormattedString(String value, InteractionContext context, DiscordRole commandTarget, DiscordRole role)
		=> value.Replace("{MENTION}", commandTarget.Mention)
			.Replace("{NAME}", commandTarget.Name)
			.Replace("{ID}", commandTarget.Id.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name)
			.Replace("{ROLE}", role.Mention)
			.Replace("{ROLENAME}", role.Name)
			.Replace("{ROLEID}", role.Id.ToString());


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context, DiscordMember commandTarget, String permission)
		=> value.Replace("{MENTION}", commandTarget.Mention)
			.Replace("{USERNAME}", commandTarget.Username)
			.Replace("{NICKNAME}", commandTarget.Nickname)
			.Replace("{ID}", commandTarget.Id.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name)
			.Replace("{PERMISSION}", permission);


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context, DiscordRole commandTarget, String permission)
			=> value.Replace("{MENTION}", commandTarget.Mention)
			.Replace("{NAME}", commandTarget.Name)
			.Replace("{ID}", commandTarget.Id.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name)
			.Replace("{PERMISSION}", permission);


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, DiscordRole role, DiscordChannel channel)
		=> value.Replace("{ROLE}", role.Mention)
			.Replace("{ROLENAME}", role.Name)
			.Replace("{ROLEID}", role.Id.ToString())
			.Replace("{CHANNEL}", channel.Mention)
			.Replace("{CHANNELNAME}", channel.Name)
			.Replace("{CHANNELID}", channel.Id.ToString());


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context)
		=> value.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, InteractionContext context, UInt64 discordMemberId)
		=> value.Replace("{ID}", discordMemberId.ToString())
			.Replace("{MODMENTION}", context.Member.Mention)
			.Replace("{MODUSERNAME}", context.Member.Username)
			.Replace("{MODNICKNAME}", context.Member.Nickname)
			.Replace("{MODID}", context.Member.Id.ToString())
			.Replace("{CHANNEL}", context.Channel.Mention)
			.Replace("{CHANNELNAME}", context.Channel.Name);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetFormattedString(String value, DiscordMember commandTarget)
		=> value.Replace("{MENTION}", commandTarget.Mention)
			   .Replace("{USERNAME}", commandTarget.Username)
			   .Replace("{NICKNAME}", commandTarget.Nickname)
			   .Replace("{ID}", commandTarget.Id.ToString());

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static String GetReason(String value, String reason) => value.Replace("{REASON}", reason);

	public static String GetMemberReason(String value, String reason, DiscordMember member)
		=> value.Replace("{REASON}", reason)
			.Replace("{MENTION}", member.Mention)
			.Replace("{NICKNAME}", member.Nickname)
			.Replace("{USERNAME}", member.Username)
			.Replace("{ID}", member.Id.ToString());
}