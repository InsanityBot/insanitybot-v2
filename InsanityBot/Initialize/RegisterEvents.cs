namespace InsanityBot;
using System;

using global::InsanityBot.Commands.Moderation;
using global::InsanityBot.Tickets.Validation;
using global::InsanityBot.Utility.Timers;

public partial class InsanityBot
{
	private static void RegisterAllEvents()
	{
		CommandsExtension.CommandErrored += PermissionFailed;

		if(Config.Value<Boolean>("insanitybot.modules.moderation"))
		{
			Timer.TimerExpiredEvent += Mute.InitializeUnmute;
			Mute.UnmuteCompletedEvent += TimeHandler.ReenableTimer;

			Timer.TimerExpiredEvent += Ban.InitializeUnban;
			Ban.UnbanCompletedEvent += TimeHandler.ReenableTimer;

			Mute.MuteStartingEvent += TimeHandler.DisableTimer;
			Ban.BanStartingEvent += TimeHandler.DisableTimer;
		}

		if(Config.Value<Boolean>("insanitybot.modules.tickets"))
		{
			Client.GuildDownloadCompleted += new TicketCacheValidator().Validate;
			Client.GuildDownloadCompleted += new TicketPermissionCacheValidator().Validate; // order is important here
			Client.ChannelDeleted += new ChannelDeleteValidator().Validate;
		}
	}
}