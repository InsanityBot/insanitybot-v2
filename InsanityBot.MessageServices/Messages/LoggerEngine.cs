using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.MessageServices.Embeds;
using InsanityBot.MessageServices.Messages.Rules;
using InsanityBot.MessageServices.Messages.Rules.Data;
using InsanityBot.MessageServices.Messages.Util;
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

            if(config.Value<Boolean>("insanitybot.modules.logging"))
            {
                if(_config.SelectToken("message_delete").Value<Boolean>())
                {
                    client.MessageDeleted += this.MessageDeleted;
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
            }

            _contextBuilder = new(_extension);
            _channelEvaluator = new();
            _commandEvaluator = new();
            _memberEvaluator = new();
            _prefixEvaluator = new();
            _embeds = embeds;

            if(!Directory.Exists("./cache/messages"))
            {
                Directory.CreateDirectory("./cache/messages");
            }

            if(!File.Exists("./config/logging.json"))
            {
                logger.LogInformation(new EventId(2100, "LoggingServices"), "Could not find logging override file, using main config values");
                CreateDefaultRules(_channels, guild);
                return;
            }

            logger.LogInformation(new EventId(2100, "LoggingServices"), "Logging override file found, generating ruleset...");
            CreateDefaultRules(_channels, guild);
            ApplyOverrides(guild);
            logger.LogInformation(new EventId(2100, "LoggingServices"), "Logging overrides were applied successfully.");
        }        

        private void CreateDefaultRules(JObject channels, DiscordGuild guild)
        {
            _rules.Defaults.Add(LogEvent.MessageDelete,
                guild.GetChannel(channels.SelectToken("message_delete_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.MessageEdit,
                guild.GetChannel(channels.SelectToken("message_edit_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.MemberJoin,
                guild.GetChannel(channels.SelectToken("member_join_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.MemberLeave,
                guild.GetChannel(channels.SelectToken("member_leave_channel").Value<UInt64>()));
            _rules.Defaults.Add(LogEvent.CommandExecution,
                guild.GetChannel(channels.SelectToken("modlog_channel").Value<UInt64>()));

            Boolean webhooks = _config.SelectToken("use_webhooks").Value<Boolean>();
            foreach(var x in _rules.Defaults.Values)
            {
                if(x != null && !_rules.Channels.ContainsKey(x.Id))
                {
                    if(webhooks)
                        _rules.Channels.Add(x.Id, new LoggingWebhook(x));
                    else
                        _rules.Channels.Add(x.Id, new LoggingChannel(x));
                }
            }
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
            ILoggingGateway gateway = GetGateway(new(ctx.Member, ctx.Channel, ctx.Command, ctx.Message), LogEvent.CommandExecution);
            await gateway.SendMessage(embed);
        }

        public async Task LogMessage(DiscordMessageBuilder messageBuilder, CommandContext ctx)
        {
            ILoggingGateway gateway = GetGateway(new(ctx.Member, ctx.Channel, ctx.Command, ctx.Message), LogEvent.CommandExecution);
            await gateway.SendMessage(messageBuilder);
        }

        private async Task CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            LogContext context = _contextBuilder.BuildContext(e);
            ILoggingGateway gateway = GetGateway(context, LogEvent.Commands);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(_config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = _embeds["insanitybot.logging.command"]
                    .AddField("Command", e.Command.Name, true)
                    .AddField("Sender", GetMemberMention(e.Context.Member), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
            else
            {
                await gateway.SendMessage($"{e.Context.Member.Username} executed command {e.Command.Name}");
            }
        }

        private async Task GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            LogContext context = _contextBuilder.BuildContext(e);
            ILoggingGateway gateway = GetGateway(context, LogEvent.MemberLeave);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(_config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = _embeds["insanitybot.logging.member_leave"]
                    .AddField("Member", GetMemberMention(e.Member), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
            else
            {
                await gateway.SendMessage($"Member {e.Member.Username} left.");
            }
        }

        private async Task GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            LogContext context = _contextBuilder.BuildContext(e);
            ILoggingGateway gateway = GetGateway(context, LogEvent.MemberJoin);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(_config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = _embeds["insanitybot.logging.member_join"]
                    .AddField("Member", GetMemberMention(e.Member), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
            else
            {
                await gateway.SendMessage($"Member {e.Member.Username} joined.");
            }
        }

        private async Task MessagesBulkDeleted(DiscordClient sender, MessageBulkDeleteEventArgs e)
        {
            LogContext context = _contextBuilder.BuildContext(e);
            ILoggingGateway gateway = GetGateway(context, LogEvent.MessageDelete);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            Guid filename = Guid.NewGuid();

            StreamWriter writer = new(File.Create($"./cache/messages/{filename}.md"));

            writer.WriteLine("# Message Transcript for bulk message deletion");
            foreach(var v in e.Messages)
            {
                writer.WriteLine($"[{v.Timestamp:dd/MM/yy-HH:mm:ss}] {v.Author.Username}: {v.Content}");
            }
            writer.Close();

            DiscordMessageBuilder builder = new();
            builder.WithFile("transcript.md", File.OpenRead($"./cache/messages/{filename}.md"));

            if(_config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = _embeds["insanitybot.logging.member_join"]
                    .AddField("Messages", e.Messages.Count.ToString(), true);
                builder.WithEmbed(embedBuilder.Build());
                await gateway.SendMessage(builder);
            }
            else
            {
                builder.WithContent($"{e.Messages.Count} messages were deleted.");
                await gateway.SendMessage(builder);
            }
        }

        private async Task MessageUpdated(DiscordClient sender, MessageUpdateEventArgs e)
        {
            LogContext context = _contextBuilder.BuildContext(e);
            ILoggingGateway gateway = GetGateway(context, LogEvent.MessageEdit);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(_config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = _embeds["insanitybot.logging.member_join"]
                    .AddField("Old", e.MessageBefore.Content, false)
                    .AddField("New", e.Message.Content, false)
                    .AddField("Link", e.Message.JumpLink.ToString(), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
        }

        private async Task MessageDeleted(DiscordClient sender, MessageDeleteEventArgs e)
        {
            LogContext context = _contextBuilder.BuildContext(e);
            ILoggingGateway gateway = GetGateway(context, LogEvent.MessageEdit);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(_config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = _embeds["insanitybot.logging.member_join"]
                    .AddField("Text", e.Message.Content, false)
                    .AddField("User", GetMemberMention(e.Message.Author), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
        }

        private ILoggingGateway GetGateway(LogContext context, LogEvent ev)
        {
            if(!_rules.Defaults.ContainsKey(ev))
            {
                return ILoggingGateway.Empty;
            }
            if(!_rules.Rules.ContainsKey(ev))
            {
                goto RETURN_DEFAULT;
            }

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
                                if(_memberEvaluator.EvaluateBotRule(context.Member, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case MemberRuleTarget.Id:
                                if(_memberEvaluator.EvaluateIdRule(context.Member, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case MemberRuleTarget.Owner:
                                if(_memberEvaluator.EvaluateOwnerRule(context.Member, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                            case MemberRuleTarget.Role:
                                if(_memberEvaluator.EvaluateRoleIdRule(context.Member, v.RuleParameter))
                                    return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                break;
                        }
                        break;
                    case RuleTarget.Command:
                        if(_commandEvaluator.EvaluateCommandRule(context.Command, v.RuleParameter))
                            return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                        break;
                    case RuleTarget.Prefix:
                        if(_prefixEvaluator.EvaluatePrefixRule(context.Prefix, v.RuleParameter))
                            return v.Allow ? _rules.Channels[v.Channel] : ILoggingGateway.Empty;
                        break;
                }
            }

            RETURN_DEFAULT:

            if(_rules.Defaults[ev] == null)
                return ILoggingGateway.Empty;
            return _rules.Channels[_rules.Defaults[ev].Id];
        }

        private String GetMemberMention(DiscordUser member)
        {
            return _config.SelectToken("members.use_mentions").Value<Boolean>() ? member.Mention :
                (_config.SelectToken("members.use_name_discriminator_format").Value<Boolean>() ?
                $"{member.Username}#{member.Discriminator}" : member.Username);
        }
    }
}
