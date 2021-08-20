using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.MessageServices.Messages.Rules;
using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

            client.MessageCreated += this.MessageCreated;
            client.MessageUpdated += this.MessageUpdated;
            client.MessagesBulkDeleted += this.MessagesBulkDeleted;
            client.GuildMemberAdded += this.GuildMemberAdded;
            client.GuildMemberRemoved += this.GuildMemberRemoved;
            commandExtension.CommandExecuted += this.CommandExecuted;

            if(!File.Exists("./config/logging.json"))
            {
                logger.LogInformation(new EventId(2100, "LoggingServices"), "Could not find logging override file, using main config values");
                CreateDefaultRules(_channels, guild);
                return;
            }

            logger.LogInformation(new EventId(2100, "LoggingServices"), "Logging override file found, generating ruleset...");
            CreateDefaultRules(_config, guild);
            ApplyOverrides(guild);
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

        private void ApplyOverrides(DiscordGuild guild)
        {
            StreamReader reader = new("./config/logging.json");
            _rules.Rules = JsonConvert.DeserializeObject<Dictionary<LogEvent, LoggerRule[]>>(reader.ReadToEnd());

            foreach(var v in _rules.Rules.Values)
            {
                foreach(var x in v)
                {
                    if(!_rules.Channels.ContainsKey(x.Channel))
                    {
                        _rules.Channels.Add(x.Channel, guild.GetChannel(x.Channel));
                    }
                }
            }
        }

        private Task CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Task GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Task GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Task MessagesBulkDeleted(DiscordClient sender, MessageBulkDeleteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Task MessageUpdated(DiscordClient sender, MessageUpdateEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
