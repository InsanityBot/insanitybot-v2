using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using DSharpPlus.Entities;

using InsanityBot.Utility.Permissions.Reference;

using Newtonsoft.Json;

namespace InsanityBot.Utility.Permissions
{
#pragma warning disable CA1822
    public class PermissionCache : ICacheHandler<UserPermissions>
    {
        public IEnumerable<UserPermissions> Cache { get; set; }

        public Task AddCacheEntry()
        {
            throw new NotImplementedException();
        }

        public Task<UserPermissions> GetCacheEntry()
        {
            throw new NotImplementedException();
        }

        public Task RemoveCacheEntry()
        {
            throw new NotImplementedException();
        }

        public Task RemoveUnusedCacheEntries()
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CA1822
}
