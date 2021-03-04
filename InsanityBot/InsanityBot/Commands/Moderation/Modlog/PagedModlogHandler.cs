using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using InsanityBot.Utility.Modlogs;

using Microsoft.Extensions.Logging;

using static InsanityBot.Commands.StringUtilities;
using static System.Convert;

namespace InsanityBot.Commands.Moderation.Modlog
{
    public partial class Modlog
    {
#pragma warning disable CS1998
        public static async Task ReactionAddedEventHandler(DiscordClient client, MessageReactionAddEventArgs args)
        {
            _ = PagedModlogHandler(args);
        }
#pragma warning restore CS1998

        private static async Task PagedModlogHandler(MessageReactionAddEventArgs args)
        {

            if (!ModlogMessageTracker.IsTracked(args.Message.Id))
                return;

            if (args.User.IsBot)
                return;

            var possibleMessages = from v in ModlogMessageTracker.MessageTracker
                                   where v.MessageId == args.Message.Id
                                   select v;

            ModlogMessageTracker.MessageTrackerEntry messageData = possibleMessages.ToList()[0];
            DiscordMessage message = await args.Channel.GetMessageAsync(possibleMessages.ToList()[0].MessageId);
            DiscordUser user = await InsanityBot.HomeGuild.GetMemberAsync(messageData.UserId);

            try
            {
                if (messageData.Type == ModlogMessageTracker.LogType.Modlog)
                {
                    if (args.Emoji == ReactionBackwards)
                    {
                        if (messageData.Page == 0)
                            goto delete_reaction;

                        DiscordEmbedBuilder modlogEmbedBuilder = new()
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                                (DiscordMember)user),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = user.CreateModlogDescription(true, ToByte(messageData.Page - 1))
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = user.Id,
                            Type = ModlogMessageTracker.LogType.Modlog,
                            Page = ToByte(messageData.Page - 1)
                        });
                    }
                    else if (args.Emoji == ReactionForwards)
                    {
                        if (messageData.Page >= ToByte(((DiscordMember)(user)).GetUserModlog().ModlogEntryCount + 1))
                            goto delete_reaction;

                        DiscordEmbedBuilder modlogEmbedBuilder = new()
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.modlog.embed_title"],
                                (DiscordMember)user),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = user.CreateModlogDescription(true, ToByte(messageData.Page + 1))
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = user.Id,
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

                        DiscordEmbedBuilder modlogEmbedBuilder = new()
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.embed_title"],
                                (DiscordMember)user),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = user.CreateVerballogDescription(true, ToByte(messageData.Page - 1))
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = user.Id,
                            Type = ModlogMessageTracker.LogType.VerbalLog,
                            Page = ToByte(messageData.Page - 1)
                        });
                    }
                    else if (args.Emoji == ReactionForwards)
                    {
                        if (messageData.Page >= ToByte(((DiscordMember)(user)).GetUserModlog().VerbalLogEntryCount + 1))
                            goto delete_reaction;

                        DiscordEmbedBuilder modlogEmbedBuilder = new()
                        {
                            Title = GetFormattedString(InsanityBot.LanguageConfig["insanitybot.commands.verbal_log.embed_title"],
                                (DiscordMember)user),
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = "InsanityBot - Exa 2020-2021"
                            },
                            Color = DiscordColor.Red,
                            Description = user.CreateModlogDescription(true, ToByte(messageData.Page + 1))
                        };

                        await message.ModifyAsync(embed: modlogEmbedBuilder.Build());

                        ModlogMessageTracker.AddTrackedMessage(new ModlogMessageTracker.MessageTrackerEntry
                        {
                            MessageId = message.Id,
                            UserId = user.Id,
                            Type = ModlogMessageTracker.LogType.VerbalLog,
                            Page = ToByte(messageData.Page + 1)
                        });
                    }
                }

            delete_reaction:
                await args.Message.DeleteReactionAsync(args.Emoji, args.User);
            }
            catch (Exception e)
            {
                InsanityBot.Client.Logger.LogError($"{e.GetType()}: {e.Message}\n{e.StackTrace}");
            }

            return;
        }

        private static DiscordEmoji ReactionForwards = null;
        private static DiscordEmoji ReactionBackwards = null;
    }
}
