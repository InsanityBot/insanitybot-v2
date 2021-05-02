using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using InsanityBot.Utility.Modlogs.Reference;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Modlogs.SafeAccessInterface
{
    public static class SafeModlogWrapper
    {
        /// <summary>
        /// Attempts to add a modlog entry to a specific user. Returns true if successful.
        /// </summary>
        public static Task<Boolean> TryAddModlogEntry(this DiscordUser user, ModlogEntry modlogEntry)
        {
            if (user == null)
            {
                throw new ArgumentException("Could not add modlog entry to nonexistent user", nameof(user));
            }

            try
            {
                (user as DiscordMember).AddModlogEntry(modlogEntry);
                return Task.FromResult(true);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Attempts to add a modlog entry to a specific user, time being inferred. Returns true if successful.
        /// </summary>
        public static Task<Boolean> TryAddModlogEntry(this DiscordUser user, ModlogEntryType type, String reason)
        {
            if(user == null)
            {
                throw new ArgumentException("Could not add modlog entry to nonexistent user", nameof(user));
            }

            try
            {
                (user as DiscordMember).AddModlogEntry(type, reason);
                return Task.FromResult(true);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Attempts to add a verballog entry to a specific user. Returns true if successful.
        /// </summary>
        public static Task<Boolean> TryAddVerballogEntry(this DiscordUser user, VerbalModlogEntry entry)
        {
            if(user == null)
            {
                throw new ArgumentException("Could not add modlog entry to nonexistent user", nameof(user));
            }

            try
            {
                (user as DiscordMember).AddVerbalModlogEntry(entry);
                return Task.FromResult(true);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Attempts to add a verballog entry to a specific user, time being inferred. Returns true if successful.
        /// </summary>
        public static Task<Boolean> TryAddVerballogEntry(this DiscordUser user, String reason)
        {
            if (user == null)
            {
                throw new ArgumentException("Could not add modlog entry to nonexistent user", nameof(user));
            }

            try
            {
                (user as DiscordMember).AddVerbalModlogEntry(reason);
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Attempts to fetch a specific user's modlog. Returns true if successful.
        /// </summary>
        /// <param name="modlog">The variable the user's modlog will be assigned to.</param>
        public static Task<Boolean> TryFetchModlog(this DiscordUser user, out UserModlog modlog)
        {
            if(user == null)
            {
                throw new ArgumentException("Could not fetch modlog of nonexistent user", nameof(user));
            }

            try
            {
                modlog = (user as DiscordMember).GetUserModlog();
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                modlog = null;
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Attempts to set a specific user's modlog. Returns true if successful.
        /// </summary>
        public static Task<Boolean> TrySetModlog(this DiscordUser user, UserModlog modlog)
        {
            if(user == null)
            {
                throw new ArgumentException("Could not set modlog of nonexistent user", nameof(user));
            }

            try
            {
                (user as DiscordMember).SetUserModlog(modlog);
                return Task.FromResult(true);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                return Task.FromResult(false);
            }
        }
    }
}
