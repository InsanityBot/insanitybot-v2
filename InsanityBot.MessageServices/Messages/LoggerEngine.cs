using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using System;
using System.IO;

namespace InsanityBot.MessageServices.Messages
{
    public class LoggerEngine
    {
        private readonly CommandsNextExtension _extension;
        private readonly ILogger<BaseDiscordClient> _logger;
        private readonly JObject _config;
        private readonly JObject _channels;
        private readonly MessageRules _rules;

        public LoggerEngine(CommandsNextExtension commandExtension, ILogger<BaseDiscordClient> logger, MainConfiguration config,
            DiscordClient client, DiscordGuild guild)
        {
            _rules = new(guild);
            this._extension = commandExtension;
            this._logger = logger;
            this._config = (JObject)config.Configuration.SelectToken("insanitybot.logging");
            this._channels = (JObject)config.Configuration.SelectToken("insanitybot.identifiers.logging");

            if(!File.Exists("./config/logging.json"))
            {
                logger.LogInformation(new EventId(2100, "LoggingServices"), "Could not find logging override file, using main config values");
                CreateDefaultRules(_channels, guild);
                return;
            }

            logger.LogInformation(new EventId(2100, "LoggingServices"), "Logging override file found, generating ruleset...");
            CreateDefaultRules(_config, guild);
            ApplyOverrides();
            logger.LogInformation(new EventId(2100, "LoggingServices"), "Logging overrides were applied successfully.");
        }

        private void CreateDefaultRules(JObject channels, DiscordGuild guild)
        {
            _rules.Defaults.Add(LogEvent.MessageDelete,
                guild.GetChannel(channels.SelectToken("message_delete_log_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.MessageEdit,
                guild.GetChannel(channels.SelectToken("message_edit_log_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.MemberJoin,
                guild.GetChannel(channels.SelectToken("member_join_log_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.MemberLeave,
                guild.GetChannel(channels.SelectToken("member_leave_log_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.CommandExecution,
                guild.GetChannel(channels.SelectToken("modlog_channel").Value<UInt64>()));
        }

        private void ApplyOverrides()
        {

        }
    }
}
