using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using InsanityBot.Utility.Config.Reference;

namespace InsanityBot.Utility.Config
{
    /// <summary>
    /// Main class for serializing and deserializing config files. Also holds a static copy of the default config values.
    /// </summary>
    public static class MainConfigManager
    {
        /// <summary>
        /// Main default configuration. Used to create a new configuration from defaults.
        /// </summary>
        public static MainConfig DefaultMainConfiguration { get; } = new MainConfig
        {
            Token = "",
            MainPrefix = new List<String>().Append("i.").ToList(),
            AdminPrefix = new List<String>().Append("i.").Append("adm.").Append("admin.").ToList(),
            CommandConfig = new CommandConfig
            {
                Toggles = new CommandToggles
                {
                    Verbalwarn = true,
                    VerbalLog = true,
                    Warn = true,
                    Unwarn = true,
                    Modlog = true,
                    Mute = true,
                    Tempmute = true,
                    Unmute = true,
                    Blacklist = true,
                    Whitelist = true,
                    Kick = true,
                    Ban = true,
                    Tempban = true,
                    Unban = true,
                    Appeal = true,
                    Slowmode = true,
                    Tag = true,
                    Faq = true,
                    Suggest = true,
                    SuggestionAccept = true,
                    SuggestionDeny = true,
                    TicketNew = true,
                    TicketReport = true,
                    TicketAdd = true,
                    TicketRemove = true,
                    TicketLeave = true,
                    TicketClose = true,
                    TicketApply = true,
                    TicketApplyAccept = true,
                    TicketApplyDeny = true,
                    Lock = true,
                    Unlock = true,
                    Archive = true,
                    Config = true,
                    Permission = true
                },

                Settings = new CommandSettings
                {
                    MinorWarnsEqualFullWarn = 3,
                    AllowCommunitySuggestionAccept = false,
                    CommunitySuggestionAcceptThreshold = 0,
                    AllowCommunitySuggestionDenial = true,
                    CommunitySuggestionDenialThreshold = 5
                }
            },
            DiscordConfig = new DiscordConfig
            {
                Identifiers = new ConfigIds
                {
                    GuildId = 0,
                    ModLogChannelId = 0,
                    TicketLogChannelId = 0,
                    ApplicationLogChannelId = 0,
                    JoinLeaveLogChannelId = 0,
                    SuggestionChannelId = 0,
                    DeniedSuggestionChannelId = 0,
                    AcceptedSuggestionChannelId = 0,
                    SuggestionUpvoteEmoteId = 0,
                    SuggestionDownvoteEmoteId = 0,
                    SuggestionAcceptEmoteId = 0,
                    SuggestionDenyEmoteId = 0,
                    TicketReactionMessageId = 0,
                    TicketReactionEmoteId = 0
                },
                Automod = true,
                AutomodWords = new List<String>(),
                AutomodLinks = true,
                WhitelistedLinks = new List<String>().Append("discord.com")
                    .Append("discordapp.com")
                    .Append("gyazo.com")
                    .Append("tenor.com")
                    .Append("giphy.com")
                    .ToList(),
                ApplicationKeys = new List<String>(),
                HandleApplicationQuestionsDms = true,
                RolePingMessages = new Dictionary<UInt64, String>()
            }
        };

        /// <summary>
        /// Reads the main configuration file and deserializes it into a MainConfig instance
        /// </summary>
        public static MainConfig DeserializeMainConfiguration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainConfig));
            StreamReader reader = new StreamReader("./config/main.xml");

            MainConfig value = (MainConfig)serializer.Deserialize(reader);
            reader.Close();
            return value;
        }

        /// <summary>
        /// Serializes the selected config to the main configuration file, overwriting existing data in the process
        /// </summary>
        /// <param name="config">Configuration instance to be serialized</param>
        public static void SerializeMainConfiguration(MainConfig config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainConfig));
            FileStream writer = new FileStream("./config/main.xml", FileMode.Truncate);

            serializer.Serialize(writer, config);
            writer.Close();
        }

        /// <summary>
        /// Serializes the default main config to the main configuration file, overwriting existing data in the process
        /// </summary>
        public static void SerializeMainConfiguration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainConfig));
            FileStream writer = new FileStream("./config/main.xml", FileMode.Truncate);

            serializer.Serialize(writer, DefaultMainConfiguration);
            writer.Close();
        }
    }
}
