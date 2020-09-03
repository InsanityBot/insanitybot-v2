using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

using DSharpPlus.Entities;

using InsanityBot.Utility.Config;
using InsanityBot.Utility.Modlogs.Reference;

namespace InsanityBot.Utility.Modlogs
{
    /// <summary>
    /// Static modlog interaction framework
    /// </summary>
    public static class Modlog
    {
        /// <summary>
        /// Serializes a modlog instance to its file
        /// </summary>
        /// <param name="user">Modlog instance to serialize</param>
        /// <param name="UserId">ID of the user whose modlog gets serialized. Used to tell apart different modlog files</param>
        private static void Serialize(UserModlog user, UInt64 UserId)
        {
            FileStream writer = new FileStream($"./data/{UserId}/modlog.xml", FileMode.Truncate);
            XmlSerializer serializer = new XmlSerializer(typeof(UserModlog));

            serializer.Serialize(writer, user);
            writer.Close();
        }

        /// <summary>
        /// Deserializes a modlog instance from its file
        /// </summary>
        /// <param name="UserId">ID of the user whose modlog gets called. Used to get the correct modlog file</param>
        /// <returns>The modlog instance of the user</returns>
        private static UserModlog Deserialize(UInt64 UserId)
        {
            StreamReader reader = new StreamReader($"./data/{UserId}/modlog.xml");
            XmlSerializer deserializer = new XmlSerializer(typeof(UserModlog));

            UserModlog returnValue = (UserModlog)deserializer.Deserialize(reader);

            reader.Close();
            return returnValue;
        }

        // extension methods for DiscordMember to allow easier accessibility

        /// <summary>
        /// Adds a new modlog entry to the file
        /// </summary>
        /// <param name="modlogEntry">The ModlogEntry instance to add to file</param>
        public static void AddModlogEntry(this DiscordMember member, ModlogEntry modlogEntry)
        {
            UserModlog user = Deserialize(member.Id);
            user.Modlog.Add(modlogEntry);
            user.ModlogEntryCount++;
            Serialize(user, member.Id);
        }

        /// <summary>
        /// Adds a new modlog entry to the file
        /// </summary>
        /// <param name="type">Modlog Type of the new entry</param>
        /// <param name="reason">Reason for the infraction</param>
        public static void AddModlogEntry(this DiscordMember member, ModlogEntryType type, String reason)
        {
            UserModlog user = Deserialize(member.Id);
            user.Modlog.Add(new ModlogEntry
            {
                Type = type,
                Time = DateTime.UtcNow,
                Reason = reason
            });
            user.ModlogEntryCount++;
            Serialize(user, member.Id);
        }

        /// <summary>
        /// Adds a new verbal log entry to the file
        /// </summary>
        /// <param name="verbalEntry">VerbalModlogEntry instance to add to file</param>
        public static void AddVerbalModlogEntry(this DiscordMember member, VerbalModlogEntry verbalEntry)
        {
            UserModlog user = Deserialize(member.Id);
            user.VerbalLog.Add(verbalEntry);
            user.VerbalLogEntryCount++;
            Serialize(user, member.Id);
        }

        /// <summary>
        /// Adds a new verbal log entry to the file
        /// </summary>
        /// <param name="reason">Reason for the infraction</param>
        public static void AddVerbalModlogEntry(this DiscordMember member, String reason)
        {
            UserModlog user = Deserialize(member.Id);
            user.VerbalLog.Add(new VerbalModlogEntry
            {
                Reason = reason,
                Time = DateTime.UtcNow
            });
            user.VerbalLogEntryCount++;
            Serialize(user, member.Id);
        }

        /// <summary>
        /// Gets this members modlog as DiscordEmbed
        /// </summary>
        public static DiscordEmbed GetModlogDiscordEmbed(this DiscordMember member)
        {
            UserModlog user = Deserialize(member.Id);
            if (user.ModlogEntryCount == 0) // user has no modlog entries
            {
                DiscordEmbedBuilder embedBuilder2 = new DiscordEmbedBuilder
                {
                    Description = MainConfigManager.DeserializeMainConfiguration().CommandConfig.Messages.NoModlogEntries,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    },
                    Color = DiscordColor.SpringGreen
                };
                return embedBuilder2;
            }


            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020"
                },
                Color = DiscordColor.DarkRed
            };
            foreach(var m in user.Modlog)
                embedBuilder.Description += $"{m.Type.ToString().ToUpper()}: {m.Time} - {m.Reason}\n"; // write all modlog entries to string

            return embedBuilder;
        }

        /// <summary>
        /// Gets this members verbal log as DiscordEmbed
        /// </summary>
        public static DiscordEmbed GetVerbalLogDiscordEmbed(this DiscordMember member)
        {
            UserModlog user = Deserialize(member.Id);
            if (user.VerbalLogEntryCount == 0) // user has no log entries
            {
                DiscordEmbedBuilder embedBuilder2 = new DiscordEmbedBuilder
                {
                    Description = MainConfigManager.DeserializeMainConfiguration().CommandConfig.Messages.NoModlogEntries,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "InsanityBot - ExaInsanity 2020"
                    },
                    Color = DiscordColor.SpringGreen
                };
                return embedBuilder2;
            }


            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "InsanityBot - ExaInsanity 2020"
                },
                Color = DiscordColor.DarkRed
            };
            foreach (var m in user.VerbalLog)
                embedBuilder.Description += $"{m.Time} - {m.Reason}\n"; // write all log entries to string

            return embedBuilder;
        }

        /// <summary>
        /// Gets the UserModlog instance of this member. Only intended for low-level data manipulation and testing, 
        /// not for inclusion in production releases.
        /// </summary>
        public static UserModlog GetUserModlog(this DiscordMember member)
            => Deserialize(member.Id);

        /// <summary>
        /// Sets the UserModlog instance of this member. Only intended for low-level data manipulation and testing,
        /// not for inclusion in production releases.
        public static void SetUserModlog(this DiscordMember member, UserModlog user)
            => Serialize(user, member.Id);
    }
}
