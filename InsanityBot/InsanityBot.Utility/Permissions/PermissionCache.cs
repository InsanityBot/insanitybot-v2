using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Serialization;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions
{
    public static class PermissionCache
    {
        private static List<UserPermission> UserPermissions { get; set; }

        private static Timer CacheHeartbeat { get; set; }

        private static void CacheHeartbeat_Elapsed(Object sender, ElapsedEventArgs e)
        {
            foreach(var p in UserPermissions)
            {
                if (p.HeartbeatsUnused == 5)
                    UserPermissions.Remove(p);
                else
                    p.HeartbeatsUnused++;
            }
        }

        /// <summary>
        /// Initializes the Timer and the Cache
        /// </summary>
        public static void Initialize()
        {
            CacheHeartbeat = new Timer
            {
                AutoReset = true,
                Interval = 30000
            };
            CacheHeartbeat.Elapsed += CacheHeartbeat_Elapsed;

            UserPermissions = new List<UserPermission>();
        }

        private static UserPermission LoadNew(UInt64 Id)
        {
            StreamReader reader = new StreamReader($"./data/{Id}/permissions.json");
            UserPermission permissions = (UserPermission)JsonConvert.DeserializeObject(reader.ReadToEnd());

            permissions.UserID = Id;
            permissions.HeartbeatsUnused = 0;

            UserPermissions.Add(permissions);
            return permissions;
        }

        // not happy with this one, someone please improve it

        /// <summary>
        /// Fetches the UserPermission object for a certain member
        /// </summary>
        public static UserPermission GetUserPermission(UInt64 Id)
        {
            var v = from vx in UserPermissions
                    where vx.UserID == Id
                    select vx;

            UserPermission returnValue = (v.Count()) switch
            {
                0 => LoadNew(Id),
                1 => v.First(),
                _ => throw new InvalidOperationException("InsanityBot.Utility.Permissions.PermissionCache.cs: Duplicate cache entries")
            };

            UserPermissions.Remove(returnValue);
            returnValue.HeartbeatsUnused = 0;
            UserPermissions.Add(returnValue);

            return returnValue;
        }

        private static void Serialize(UserPermission permissions)
        {
            FileStream file = new FileStream($"./data/{permissions.UserID}/permissions.json", FileMode.Truncate);
            StreamWriter writer = new StreamWriter(file);

            writer.Write(JsonConvert.SerializeObject(permissions));
        }

        /// <summary>
        /// Fetches the UserPermission object for this member.
        /// </summary>
        public static UserPermission GetPermissions(this DiscordMember member)
        {
            var v = from vx in UserPermissions
                    where vx.UserID == member.Id
                    select vx;

            UserPermission returnValue = (v.Count()) switch
            {
                0 => LoadNew(member.Id),
                1 => v.First(),
                _ => throw new InvalidOperationException("InsanityBot.Utility.Permissions.PermissionCache.cs: Duplicate cache entries")
            };

            UserPermissions.Remove(returnValue);
            returnValue.HeartbeatsUnused = 0;
            UserPermissions.Add(returnValue);

            return returnValue;
        }

        public static void SetPermissions(this DiscordMember member, UserPermission permissions)
        {
            var v = from vx in UserPermissions
                    where vx.UserID == member.Id
                    select vx;

            switch (v.Count())
            {
                case 0:
                    Serialize(permissions);
                    return;
                case 1:
                    UserPermissions.Remove(v.First());
                    return;
                default:
                    throw new InvalidOperationException("InsanityBot.Utility.Permissions.PermissionCache.cs: Duplicate cache entries");
            }
        }
    }
}
