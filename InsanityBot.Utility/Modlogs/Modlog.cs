using DSharpPlus.Entities;

using InsanityBot.Utility.Exceptions;
using InsanityBot.Utility.Modlogs.Reference;

using Newtonsoft.Json;

using System;
using System.IO;

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
            FileStream file = new($"./data/{UserId}/modlog.json", FileMode.Truncate);
            StreamWriter writer = new(file);

            writer.Write(JsonConvert.SerializeObject(user, Formatting.Indented));

            writer.Close();
        }

        /// <summary>
        /// Deserializes a modlog instance from its file
        /// </summary>
        /// <param name="UserId">ID of the user whose modlog gets called. Used to get the correct modlog file</param>
        /// <returns>The modlog instance of the user</returns>
        private static UserModlog Deserialize(String UserName, UInt64 UserId)
        {
            if (!File.Exists($"./data/{UserId}/modlog.json"))
            {
                Create(UserName, UserId);
            }

            StreamReader reader = new($"./data/{UserId}/modlog.json");
            String text = reader.ReadToEnd();
            reader.Close();

            UserModlog modlog = JsonConvert.DeserializeObject<UserModlog>(text);
            modlog.Username = UserName;

            StreamWriter writer = new($"./data/{UserId}/modlog.json");
            writer.Write(JsonConvert.SerializeObject(modlog, Formatting.Indented));
            writer.Close();

            return modlog;
        }

        public static UserModlog Create(String UserName, UInt64 UserId)
        {
            if (!Directory.Exists($"./data/{UserId}"))
            {
                Directory.CreateDirectory($"./data/{UserId}");
            }

            if (!File.Exists($"./data/{UserId}/modlog.json"))
            {
                StreamWriter writer = new(File.Create($"./data/{UserId}/modlog.json"));
                UserModlog modlog = new(UserName);

                writer.Write(JsonConvert.SerializeObject(modlog, Formatting.Indented));
                writer.Close();
                return modlog;
            }
            return null;
        }

        // extension methods for DiscordMember to allow easier accessibility

        /// <summary>
        /// Adds a new modlog entry to the file
        /// </summary>
        /// <param name="modlogEntry">The ModlogEntry instance to add to file</param>
        public static void AddModlogEntry(this DiscordMember member, ModlogEntry modlogEntry)
        {
            UserModlog user = Deserialize(member.Username, member.Id);
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
            try
            {
                UserModlog user = Deserialize(member.Username, member.Id);
                if (user == null)
                {
                    throw new ModlogNotFoundException("Invalid modlog file", member.Id);
                }

                user.Modlog.Add(new ModlogEntry
                {
                    Type = type,
                    Time = DateTime.UtcNow,
                    Reason = reason
                });
                user.ModlogEntryCount++;
                Serialize(user, member.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Adds a new verbal log entry to the file
        /// </summary>
        /// <param name="verbalEntry">VerbalModlogEntry instance to add to file</param>
        public static void AddVerbalModlogEntry(this DiscordMember member, VerbalModlogEntry verbalEntry)
        {
            UserModlog user = Deserialize(member.Username, member.Id);
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
            UserModlog user = Deserialize(member.Username, member.Id);
            user.VerbalLog.Add(new VerbalModlogEntry
            {
                Reason = reason,
                Time = DateTime.UtcNow
            });
            user.VerbalLogEntryCount++;
            Serialize(user, member.Id);
        }

        /// <summary>
        /// Gets the UserModlog instance of this member. Only intended for low-level data manipulation and testing, 
        /// not for inclusion in production releases.
        /// </summary>
        public static UserModlog GetUserModlog(this DiscordMember member)
            => Deserialize(member.Username, member.Id);

        /// <summary>
        /// Sets the UserModlog instance of this member. Only intended for low-level data manipulation and testing,
        /// not for inclusion in production releases.
        /// </summary>
        public static void SetUserModlog(this DiscordMember member, UserModlog user)
            => Serialize(user, member.Id);
    }
}
