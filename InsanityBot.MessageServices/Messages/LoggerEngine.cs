using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

namespace InsanityBot.MessageServices.Messages
{
    public class LoggerEngine
    {
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
            this._rules = new(guild);
            this._config = (JObject)config.Configuration.SelectToken("insanitybot.logging");
            this._channels = (JObject)config.Configuration.SelectToken("insanitybot.identifiers.logging");

            if(config.Value<Boolean>("insanitybot.modules.logging"))
            {
                if(this._config.SelectToken("message_delete").Value<Boolean>())
                {
                    client.MessageDeleted += this.MessageDeleted;
                    client.MessagesBulkDeleted += this.MessagesBulkDeleted;
                }
                if(this._config.SelectToken("message_edit").Value<Boolean>())
                {
                    client.MessageUpdated += this.MessageUpdated;
                }
                if(this._config.SelectToken("member_join").Value<Boolean>())
                {
                    client.GuildMemberAdded += this.GuildMemberAdded;
                }
                if(this._config.SelectToken("member_leave").Value<Boolean>())
                {
                    client.GuildMemberRemoved += this.GuildMemberRemoved;
                }
                if(this._config.SelectToken("commands").Value<Boolean>())
                {
                    commandExtension.CommandExecuted += this.CommandExecuted;
                }
            }

            this._contextBuilder = new();
            this._channelEvaluator = new();
            this._commandEvaluator = new();
            this._memberEvaluator = new();
            this._prefixEvaluator = new();
            this._embeds = embeds;

            if(!Directory.Exists("./cache/messages"))
            {
                Directory.CreateDirectory("./cache/messages");
            }

            if(!File.Exists("./config/logging.json"))
            {
                logger.LogInformation(new EventId(2100, "LoggingServices"), "Could not find logging override file, using main config values");
                this.CreateDefaultRules(this._channels, guild);
                return;
            }

            logger.LogInformation(new EventId(2100, "LoggingServices"), "Logging override file found, generating ruleset...");
            this.CreateDefaultRules(this._channels, guild);
            this.ApplyOverrides(guild);
            logger.LogInformation(new EventId(2100, "LoggingServices"), "Logging overrides were applied successfully.");
        }

        private void CreateDefaultRules(JObject channels, DiscordGuild guild)
        {
            this._rules.Defaults.Add(LogEvent.MessageDelete,
                guild.GetChannel(channels.SelectToken("message_delete_channel").Value<UInt64>()));
            this._rules.Defaults.Add(LogEvent.MessageEdit,
                guild.GetChannel(channels.SelectToken("message_edit_channel").Value<UInt64>()));
            this._rules.Defaults.Add(LogEvent.MemberJoin,
                guild.GetChannel(channels.SelectToken("member_join_channel").Value<UInt64>()));
            this._rules.Defaults.Add(LogEvent.MemberLeave,
                guild.GetChannel(channels.SelectToken("member_leave_channel").Value<UInt64>()));
            this._rules.Defaults.Add(LogEvent.CommandExecution,
                guild.GetChannel(channels.SelectToken("modlog_channel").Value<UInt64>()));

            Boolean webhooks = this._config.SelectToken("use_webhooks").Value<Boolean>();
            foreach(DiscordChannel x in this._rules.Defaults.Values)
            {
                if(x != null && !this._rules.Channels.ContainsKey(x.Id))
                {
                    if(webhooks)
                    {
                        this._rules.Channels.Add(x.Id, new LoggingWebhook(x));
                    }
                    else
                    {
                        this._rules.Channels.Add(x.Id, new LoggingChannel(x));
                    }
                }
            }
        }

        private void ApplyOverrides(DiscordGuild guild)
        {
            StreamReader reader = new("./config/logging.json");
            this._rules.Rules = JsonConvert.DeserializeObject<Dictionary<LogEvent, LoggerRule[]>>(reader.ReadToEnd());

            Boolean webhooks = this._config.SelectToken("use_webhooks").Value<Boolean>();

            foreach(LoggerRule[] v in this._rules.Rules.Values)
            {
                foreach(LoggerRule x in v)
                {
                    if(!this._rules.Channels.ContainsKey(x.Channel))
                    {
                        if(webhooks)
                        {
                            this._rules.Channels.Add(x.Channel, new LoggingWebhook(guild.GetChannel(x.Channel)));
                        }
                        else
                        {
                            this._rules.Channels.Add(x.Channel, new LoggingChannel(guild.GetChannel(x.Channel)));
                        }
                    }
                }
            }
        }

        public async Task LogMessage(DiscordEmbed embed, CommandContext ctx)
        {
            ILoggingGateway gateway = this.GetGateway(new(ctx.Member, ctx.Channel, ctx.Command, ctx.Message), LogEvent.CommandExecution);
            await gateway.SendMessage(embed);
        }

        public async Task LogMessage(DiscordMessageBuilder messageBuilder, CommandContext ctx)
        {
            ILoggingGateway gateway = this.GetGateway(new(ctx.Member, ctx.Channel, ctx.Command, ctx.Message), LogEvent.CommandExecution);
            await gateway.SendMessage(messageBuilder);
        }

        private async Task CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            LogContext context = this._contextBuilder.BuildContext(e);
            ILoggingGateway gateway = this.GetGateway(context, LogEvent.Commands);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(this._config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = this._embeds["insanitybot.logging.command"]
                    .AddField("Command", e.Command.Name, true)
                    .AddField("Sender", this.GetMemberMention(e.Context.Member), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
            else
            {
                await gateway.SendMessage($"{e.Context.Member.Username} executed command {e.Command.Name}");
            }
        }

        private async Task GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            LogContext context = this._contextBuilder.BuildContext(e);
            ILoggingGateway gateway = this.GetGateway(context, LogEvent.MemberLeave);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(this._config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = this._embeds["insanitybot.logging.member_leave"]
                    .AddField("Member", this.GetMemberMention(e.Member), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
            else
            {
                await gateway.SendMessage($"Member {e.Member.Username} left.");
            }
        }

        private async Task GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            LogContext context = this._contextBuilder.BuildContext(e);
            ILoggingGateway gateway = this.GetGateway(context, LogEvent.MemberJoin);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(this._config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = this._embeds["insanitybot.logging.member_join"]
                    .AddField("Member", this.GetMemberMention(e.Member), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
            else
            {
                await gateway.SendMessage($"Member {e.Member.Username} joined.");
            }
        }

        private async Task MessagesBulkDeleted(DiscordClient sender, MessageBulkDeleteEventArgs e)
        {
            LogContext context = this._contextBuilder.BuildContext(e);
            ILoggingGateway gateway = this.GetGateway(context, LogEvent.MessageDelete);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            Guid filename = Guid.NewGuid();

            StreamWriter writer = new(File.Create($"./cache/messages/{filename}.md"));

            writer.WriteLine("# Message Transcript for bulk message deletion");
            foreach(DiscordMessage v in e.Messages)
            {
                writer.WriteLine($"[{v.Timestamp:dd/MM/yy-HH:mm:ss}] {v.Author.Username}: {v.Content}");
            }
            writer.Close();

            DiscordMessageBuilder builder = new();
            builder.WithFile("transcript.md", File.OpenRead($"./cache/messages/{filename}.md"));

            if(this._config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = this._embeds["insanitybot.logging.member_join"]
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
            LogContext context = this._contextBuilder.BuildContext(e);
            ILoggingGateway gateway = this.GetGateway(context, LogEvent.MessageEdit);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(this._config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = this._embeds["insanitybot.logging.member_join"]
                    .AddField("Old", e.MessageBefore.Content, false)
                    .AddField("New", e.Message.Content, false)
                    .AddField("Link", e.Message.JumpLink.ToString(), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
        }

        private async Task MessageDeleted(DiscordClient sender, MessageDeleteEventArgs e)
        {
            LogContext context = this._contextBuilder.BuildContext(e);
            ILoggingGateway gateway = this.GetGateway(context, LogEvent.MessageEdit);

            if(gateway == ILoggingGateway.Empty)
            {
                return;
            }

            if(this._config.SelectToken("use_embeds").Value<Boolean>())
            {
                DiscordEmbedBuilder embedBuilder = this._embeds["insanitybot.logging.member_join"]
                    .AddField("Text", e.Message.Content, false)
                    .AddField("User", this.GetMemberMention(e.Message.Author), true);
                await gateway.SendMessage(embedBuilder.Build());
            }
        }

        private ILoggingGateway GetGateway(LogContext context, LogEvent ev)
        {
            if(!this._rules.Defaults.ContainsKey(ev))
            {
                return ILoggingGateway.Empty;
            }
            if(!this._rules.Rules.ContainsKey(ev))
            {
                goto RETURN_DEFAULT;
            }

            foreach(LoggerRule v in this._rules.Rules[ev])
            {
                switch(v.Target)
                {
                    case RuleTarget.Channel:
                        switch(v.ChannelTarget)
                        {
                            case ChannelRuleTarget.Category:
                                if(this._channelEvaluator.EvaluateCategoryRule(context.Channel, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                            case ChannelRuleTarget.FullName:
                                if(this._channelEvaluator.EvaluateFullNameRule(context.Channel, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                            case ChannelRuleTarget.Id:
                                if(this._channelEvaluator.EvaluateIdRule(context.Channel, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                            case ChannelRuleTarget.NameContains:
                                if(this._channelEvaluator.EvaluateNameContainsRule(context.Channel, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                            case ChannelRuleTarget.NameStartsWith:
                                if(this._channelEvaluator.EvaluateNameStartsWithRule(context.Channel, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                        }
                        break;
                    case RuleTarget.Member:
                        switch(v.MemberTarget)
                        {
                            case MemberRuleTarget.Bot:
                                if(this._memberEvaluator.EvaluateBotRule(context.Member, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                            case MemberRuleTarget.Id:
                                if(this._memberEvaluator.EvaluateIdRule(context.Member, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                            case MemberRuleTarget.Owner:
                                if(this._memberEvaluator.EvaluateOwnerRule(context.Member, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                            case MemberRuleTarget.Role:
                                if(this._memberEvaluator.EvaluateRoleIdRule(context.Member, v.RuleParameter))
                                {
                                    return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                                }

                                break;
                        }
                        break;
                    case RuleTarget.Command:
                        if(this._commandEvaluator.EvaluateCommandRule(context.Command, v.RuleParameter))
                        {
                            return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                        }

                        break;
                    case RuleTarget.Prefix:
                        if(this._prefixEvaluator.EvaluatePrefixRule(context.Prefix, v.RuleParameter))
                        {
                            return v.Allow ? this._rules.Channels[v.Channel] : ILoggingGateway.Empty;
                        }

                        break;
                }
            }

        RETURN_DEFAULT:

            if(this._rules.Defaults[ev] == null)
            {
                return ILoggingGateway.Empty;
            }

            return this._rules.Channels[this._rules.Defaults[ev].Id];
        }

        private String GetMemberMention(DiscordUser member)
        {
            return this._config.SelectToken("members.use_mentions").Value<Boolean>() ? member.Mention :
                (this._config.SelectToken("members.use_name_discriminator_format").Value<Boolean>() ?
                $"{member.Username}#{member.Discriminator}" : member.Username);
        }
    }
}
