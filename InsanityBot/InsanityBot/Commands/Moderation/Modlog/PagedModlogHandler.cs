using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;
using InsanityBot.Utility.Modlogs;
using Microsoft.Extensions.Logging;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public partial class Modlog
    {
        public static async Task ReactionAddedEventHandler(DiscordClient client, MessageReactionAddEventArgs args)
        {
            if (!ModlogMessageTracker.IsTracked(args.Message.Id))
                return;

            var possibleMessages = from v in ModlogMessageTracker.MessageTracker
                                   where v.MessageId == args.Message.Id
                                   select v;

            ModlogMessageTracker.MessageTrackerEntry messageData = possibleMessages.ToList()[0];
            DiscordMessage message = await args.Channel.GetMessageAsync(possibleMessages.ToList()[0].MessageId);

            try
            {
                if (messageData.Type == ModlogMessageTracker.LogType.Modlog)
                {
                    if (args.Emoji == ReactionBackwards)
                    {
                        if (messageData.Page == 0)
                            goto delete_reaction;

                        DiscordEmbedBuilder modlogEmbedBuilder = new DiscordEmbedBuilder
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                                (DiscordMember)message.Author),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = message.Author.GetModlogEntries(ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]),
                                ToByte(messageData.Page - 1))
                                .ConvertToString()
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = message.Author.Id,
                            Type = ModlogMessageTracker.LogType.Modlog,
                            Page = ToByte(messageData.Page - 1)
                        });
                    }
                    else if (args.Emoji == ReactionForwards)
                    {
                        if (messageData.Page >= ToByte(((DiscordMember)(message.Author)).GetUserModlog().ModlogEntryCount + 1))
                            goto delete_reaction;

                        DiscordEmbedBuilder modlogEmbedBuilder = new DiscordEmbedBuilder
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                                (DiscordMember)message.Author),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = message.Author.GetModlogEntries(ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_modlog_entries_per_embed"]),
                                ToByte(messageData.Page + 1))
                                .ConvertToString()
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = message.Author.Id,
                            Type = ModlogMessageTracker.LogType.Modlog,
                            Page = ToByte(messageData.Page + 1)
                        });
                    }
                }
                else
                {
                    if (args.Emoji == ReactionBackwards)
                    {
                        if (messageData.Page == 0)
                            goto delete_reaction;

                        DiscordEmbedBuilder modlogEmbedBuilder = new DiscordEmbedBuilder
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.embed_title"],
                                (DiscordMember)message.Author),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = message.Author.GetVerballogEntries(ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]),
                                ToByte(messageData.Page - 1))
                                .ConvertToString()
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = message.Author.Id,
                            Type = ModlogMessageTracker.LogType.VerbalLog,
                            Page = ToByte(messageData.Page - 1)
                        });
                    }
                    else if (args.Emoji == ReactionForwards)
                    {
                        if (messageData.Page >= ToByte(((DiscordMember)(message.Author)).GetUserModlog().VerbalLogEntryCount + 1))
                            goto delete_reaction;

                        DiscordEmbedBuilder modlogEmbedBuilder = new DiscordEmbedBuilder
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.embed_title"],
                                (DiscordMember)message.Author),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = message.Author.GetVerballogEntries(ToByte(InsanityBot.Config["insanitybot.commands.modlog.max_verballog_entries_per_embed"]),
                                ToByte(messageData.Page + 1))
                                .ConvertToString()
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = message.Author.Id,
                            Type = ModlogMessageTracker.LogType.VerbalLog,
                            Page = ToByte(messageData.Page + 1)
                        });
                    }
                }

delete_reaction:
                await args.Message.DeleteReactionAsync(args.Emoji, args.User);
            }
            catch(Exception e)
            {
                InsanityBot.Client.Logger.LogError($"{e.GetType()}: {e.Message}\n{e.StackTrace}");
            }

            return;
        }

        private static DiscordEmoji ReactionForwards = null;
        private static DiscordEmoji ReactionBackwards = null;
    }
}
