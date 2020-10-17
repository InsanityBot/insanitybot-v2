using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Reference;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions
{
#pragma warning disable CA1822
#pragma warning disable CS1998
    public class PermissionCache : ICacheHandler<UserPermissions, UInt64>, ITimedCacheHandler
    {
        // should be used as a list - only instantiate this as List<UserPermissions> when using this class
        public IEnumerable<UserPermissions> Cache { get; set; }
        public System.Timers.Timer Timer { get; set; }
        private Boolean CacheLocked { get; set; }

        public async Task<UserPermissions> AddCacheEntry(UInt64 Id)
        {
            if (File.Exists($"./data/{Id}/permissions.json"))
            {
                StreamReader reader = new StreamReader(File.OpenRead($"./data/{Id}/permissions.json"));
                UserPermissions permissions = JsonConvert.DeserializeObject<UserPermissions>(reader.ReadToEnd());
                Cache.Append(permissions);
                return permissions;
            }

            StreamWriter writer = new StreamWriter(File.Open($"./data/{Id}/permissions.json", FileMode.Truncate));
            UserPermissions NewPermissions = new UserPermissions(Id);

            Cache.Append(NewPermissions);
            writer.Write(JsonConvert.SerializeObject(NewPermissions));
            return NewPermissions;
        }

        public async Task<UserPermissions> GetCacheEntry(UInt64 Id)
        {
            UserPermissions c = null;
            while (CacheLocked)
                Thread.Sleep(10);
            if((c = Cache.FirstOrDefault((p) => p.SnowflakeIdentifier == Id)) != null)
                return c;

            return await AddCacheEntry(Id);
        }

        public async Task RemoveCacheEntry(UserPermissions entry)
        {
            Cache.ToList().Remove(entry);
        }

        public async Task RemoveUnusedCacheEntries()
        {
            CacheLocked = true;
            TimeSpan expiration = new TimeSpan(0, 4, 0);
            DateTime current = DateTime.UtcNow;
            foreach(var v in Cache)
            {
                if (v.LastUsedAt.Subtract(current) >= expiration)
                    await RemoveCacheEntry(v);
            }
            CacheLocked = false;
        }

        public void InitializeTimer()
        {
            /* Timer will fire every 16 seconds, permission cache has a default lifetime of 4 minutes. 
             * Theres no particular reason to these numbers as the cache entries
             * are created independently of the timer, there is no reason at all to sync those.
             * 
             * AutoReset is disabled to prevent corruption - the next timer cycle will be started as soon
             * as the cache has been cleared, and more importantly all serialization is done. */
            Timer = new System.Timers.Timer()
            {
                Interval = 16000,
                AutoReset = false
            };
            Timer.Elapsed += OnTimerElapsed;
            Timer.Start();
        }

        public async void OnTimerElapsed(Object sender, ElapsedEventArgs e)
        {
            // awaiting this call will ensure the cache is in a workable state again when the timer starts again
            await RemoveUnusedCacheEntries();
            Timer.Start();
        }

        public PermissionCache()
        {
            Cache = new List<UserPermissions>();
            CacheLocked = false;
            InitializeTimer();
        }
    }
#pragma warning restore CS1998
#pragma warning restore CA1822
}
