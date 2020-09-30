using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InsanityBot.Utility
{
    interface ICacheHandler<T> where T : ICacheable
    {
        public IEnumerable<T> Cache { get; set; }

        public Task<T> GetCacheEntry();
        public Task RemoveUnusedCacheEntries();
        public Task AddCacheEntry();
        public Task RemoveCacheEntry();
    }
}
