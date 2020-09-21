using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

namespace InsanityBot
{
    public partial class InsanityBot
    {
        public static MainConfig Config;

        public static DiscordConfiguration ClientConfiguration;
        public static DiscordClient Client;

        public static DiscordGuild HomeGuild;

        public static CommandsNextExtension CommandsExtension;
        public static CommandsNextConfiguration CommandConfiguration;

        //discord roles which are used as defaults

        //discord channels which are used as defaults
        public static DiscordChannel ModlogChannel;
        public static DiscordChannel TicketLogChannel;
        public static DiscordChannel ApplicationLogChannel;
        public static DiscordChannel JoinLeaveLogChannel;
        public static DiscordChannel MessageLogChannel;

        public static DiscordChannel SuggestionChannel;
        public static DiscordChannel AcceptedSuggestionChannel;
        public static DiscordChannel DeniedSuggestionChannel;

        //discord emotes which are used as defaults
        public static DiscordEmoji SuggestionUpvoteEmote;
        public static DiscordEmoji SuggestionDownvoteEmote;
        public static DiscordEmoji SuggestionAcceptEmote;
        public static DiscordEmoji SuggestionDenyEmote;

        //discord channel categories which are used as defaults - technically a discord channel but we dont want confusion
        public static DiscordChannel TicketChannelCategory;

        private static async Task InitializeDefaultObjects()
        {
            Client.Logger.LogInformation("You might now get some more or less nasty warnings. If you did not define IDs for those channels you can safely ignore them.");

            #region log channels
            //modlog channel initialization
            if (Config.DiscordConfig.Identifiers.ModLogChannelId == 0)
                Client.Logger.LogWarning("Modlog Channel ID is not defined, moderation actions will not be logged.");
            else
                ModlogChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.ModLogChannelId);

            //ticket log channel initialization
            if (Config.DiscordConfig.Identifiers.TicketLogChannelId == 0)
                Client.Logger.LogWarning("Ticketlog Channel ID is not defined, ticket actions will not be logged.");
            else
                TicketLogChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.TicketLogChannelId);

            //app log channel initialization
            if (Config.DiscordConfig.Identifiers.ApplicationLogChannelId == 0)
                Client.Logger.LogWarning("Application Log Channel ID is not defined, application actions will not be logged.");
            else
                ApplicationLogChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.ApplicationLogChannelId);

            //join-leave log channel initialization
            if (Config.DiscordConfig.Identifiers.JoinLeaveLogChannelId == 0)
                Client.Logger.LogWarning("Join-Leave Log Channel ID is not defined, member joins and leaves will not be logged.");
            else
                JoinLeaveLogChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.JoinLeaveLogChannelId);

            //message log channel initialization
            if (Config.DiscordConfig.Identifiers.MessageLogChannelId == 0)
                Client.Logger.LogWarning("Message Log Channel ID is not defined, message edits and deletes will not be logged.");
            else
                MessageLogChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.MessageLogChannelId);
            #endregion

            #region suggestions
            //suggestion channel initialization
            if (Config.DiscordConfig.Identifiers.SuggestionChannelId == 0)
                Client.Logger.LogWarning("Suggestion Channel ID is not defined, if you have suggestions enabled using the command will cause trouble.");
            else
                SuggestionChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.SuggestionChannelId);

            //denied suggestion channel initialization
            if (Config.DiscordConfig.Identifiers.DeniedSuggestionChannelId == 0)
                Client.Logger.LogWarning("Denied Suggestion Channel ID is not defined, if you have suggestions enabled denying a suggestion will cause trouble.");
            else
                DeniedSuggestionChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.DeniedSuggestionChannelId);

            //accepted suggestion channel initialization
            if (Config.DiscordConfig.Identifiers.AcceptedSuggestionChannelId == 0)
                Client.Logger.LogWarning("Accepted Suggestion Channel ID is not defined, if you have suggestions enabled accepting a suggestion will cause trouble.");
            else
                AcceptedSuggestionChannel = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.AcceptedSuggestionChannelId);

            //suggestion downvote emote initialization
            if (Config.DiscordConfig.Identifiers.SuggestionDownvoteEmoteId == 0)
                Client.Logger.LogWarning("Suggestion Downvote Emote ID is not defined, if you have suggestions enabled using the command will cause trouble.");
            else
                SuggestionDownvoteEmote = await HomeGuild.GetEmojiAsync(Config.DiscordConfig.Identifiers.SuggestionDownvoteEmoteId);

            //suggestion upvote emote initialization
            if (Config.DiscordConfig.Identifiers.SuggestionUpvoteEmoteId == 0)
                Client.Logger.LogWarning("Suggestion Upvote Emote ID is not defined, if you have suggestions enabled using the command will cause trouble.");
            else
                SuggestionUpvoteEmote = await HomeGuild.GetEmojiAsync(Config.DiscordConfig.Identifiers.SuggestionUpvoteEmoteId);

            //suggestion deny emote initialization
            if (Config.DiscordConfig.Identifiers.SuggestionDenyEmoteId == 0)
                Client.Logger.LogWarning("Suggestion Deny Emote ID is not defined, if you have suggestions enabled using the command will cause trouble.");
            else
                SuggestionDenyEmote = await HomeGuild.GetEmojiAsync(Config.DiscordConfig.Identifiers.SuggestionDenyEmoteId);

            //suggestion accept emote initialization
            if (Config.DiscordConfig.Identifiers.SuggestionAcceptEmoteId == 0)
                Client.Logger.LogWarning("Suggestion Accept Emote ID is not defined, if you have suggestions enabled using the command will cause trouble.");
            else
                SuggestionAcceptEmote = await HomeGuild.GetEmojiAsync(Config.DiscordConfig.Identifiers.SuggestionAcceptEmoteId);
            #endregion

            //ticket category id
            if (Config.DiscordConfig.Identifiers.TicketCategoryId == 0)
                Client.Logger.LogWarning("Ticket Category ID is not defined, using a ticket command will cause trouble.");
            else
                TicketChannelCategory = HomeGuild.GetChannel(Config.DiscordConfig.Identifiers.TicketCategoryId);
        }
    }
}
