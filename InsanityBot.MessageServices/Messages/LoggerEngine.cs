using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.MessageServices.Embeds;
using InsanityBot.MessageServices.Messages.Rules;
using InsanityBot.MessageServices.Messages.Rules.Data;
using InsanityBot.Utility.Config;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly FakeContextBuilder _contextBuilder;
        private readonly ChannelRuleEvaluator _channelEvaluator;
        private readonly CommandRuleEvaluator _commandEvaluator;
        private readonly MemberRuleEvaluator _memberEvaluator;
        private readonly PrefixRuleEvaluator _prefixEvaluator;
        private readonly EmbedHandler _embeds;

        public LoggerEngine(CommandsNextExtension commandExtension, ILogger<BaseDiscordClient> logger, MainConfiguration config,
            DiscordClient client, DiscordGuild guild, EmbedHandler embeds)
        {
            _rules = new(guild);
            this._extension = commandExtension;
            this._logger = logger;
            this._config = (JObject)config.Configuration.SelectToken("insanitybot.logging");
            this._channels = (JObject)config.Configuration.SelectToken("insanitybot.identifiers.logging");

            if(_config.SelectToken("message_delete").Value<Boolean>())
            {
                client.MessageCreated += this.MessageCreated;
                client.MessagesBulkDeleted += this.MessagesBulkDeleted;
            }
            if(_config.SelectToken("message_edit").Value<Boolean>())
            {
                client.MessageUpdated += this.MessageUpdated;
            }
            if(_config.SelectToken("member_join").Value<Boolean>())
            {
                client.GuildMemberAdded += this.GuildMemberAdded;
            }
            if(_config.SelectToken("member_leave").Value<Boolean>())
            {
                client.GuildMemberRemoved += this.GuildMemberRemoved;
            }
            if(_config.SelectToken("commands").Value<Boolean>())
            {
                commandExtension.CommandExecuted += this.CommandExecuted;
            }

            _contextBuilder = new(_extension);
            _channelEvaluator = new();
            _commandEvaluator = new();
            _memberEvaluator = new();
            _prefixEvaluator = new();
            _embeds = embeds;

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

            Boolean webhooks = _config.SelectToken("use_webhooks").Value<Boolean>();

            foreach(var v in _rules.Rules.Values)
            {
                foreach(var x in v)
                {
                    if(!_rules.Channels.ContainsKey(x.Channel))
                    {
                        if(webhooks)
                            _rules.Channels.Add(x.Channel, new LoggingWebhook(guild.GetChannel(x.Channel)));
                        else
                            _rules.Channels.Add(x.Channel, new LoggingChannel(guild.GetChannel(x.Channel)));
                    }
                }
            }
        }

        public async Task LogMessage(DiscordEmbed embed, CommandContext ctx)
        {
            ILoggingGateway gateway = GetGateway(ctx, LogEvent.CommandExecution);
            await gateway.SendMessage(embed);
        }

        public async Task LogMessage(DiscordMessageBuilder messageBuilder, CommandContext ctx)
        {
            ILoggingGateway gateway = GetGateway(ctx, LogEvent.CommandExecution);
            await gateway.SendMessage(messageBuilder);
        }

        private async Task CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            CommandContext context = _contextBuilder.BuildContext(e);
            ILoggingGateway gateway = GetGateway(context, LogEvent.Commands);

            if(_config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = _embeds["insanitybot.logging.command"]
                    .AddField("Command", e.Command.Name, true)
                    .AddField("Sender", _config.SelectToken("members.use_mentions").Value<Boolean>() ? e.Context.Member.Mention :
                        (_config.SelectToken("members.use_name_discriminator_format").Value<Boolean>() ?
                        $"{e.Context.Member.Username}#{e.Context.Member.Discriminator}" : e.Context.Member.Username), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
            else
            {
                await gateway.SendMessage($"{e.Context.Member.Username} executed command {e.Command.Name}");
            }
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

        private ILoggingGateway GetGateway(CommandContext context, LogEvent ev)
        {
            foreach(var v in _rules.Rules[ev])
            {
                switch(v.Target)
                {
                    case RuleTarget.Channel:
                        switch(v.ChannelTarget)
                        {
                            case ChannelRuleTarget.Category:
                                if(_channelEvaluator.EvaluateCategoryRule(context.Channel, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case ChannelRuleTarget.FullName:
                                if(_channelEvaluator.EvaluateFullNameRule(context.Channel, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case ChannelRuleTarget.Id:
                                if(_channelEvaluator.EvaluateIdRule(context.Channel, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case ChannelRuleTarget.NameContains:
                                if(_channelEvaluator.EvaluateNameContainsRule(context.Channel, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case ChannelRuleTarget.NameStartsWith:
                                if(_channelEvaluator.EvaluateNameStartsWithRule(context.Channel, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                        }
                        break;
                    case RuleTarget.Member:
                        switch(v.MemberTarget)
                        {
                            case MemberRuleTarget.Bot:
                                if(_memberEvaluator.EvaluateBotRule(context, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case MemberRuleTarget.Id:
                                if(_memberEvaluator.EvaluateIdRule(context, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case MemberRuleTarget.Owner:
                                if(_memberEvaluator.EvaluateOwnerRule(context, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case MemberRuleTarget.Role:
                                if(_memberEvaluator.EvaluateRoleIdRule(context, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                        }
                        break;
                    case RuleTarget.Command:
                        if(_commandEvaluator.EvaluateCommandRule(context, v.RuleParameter))
                            return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                        break;
                    case RuleTarget.Prefix:
                        if(_prefixEvaluator.EvaluatePrefixRule(context, v.RuleParameter))
                            return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                        break;
                }
            }
            return _rules.Channels[_rules.Defaults[ev].Id];
        }
    }
}
