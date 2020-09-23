using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InsanityBot.Utility
{
    interface ICacheHandler
    {
        public IEnumerable<ICacheable> Cache { get; protected set; }
        protected Timer CacheIterationTimer { get; set; }

        public Task<IEnumerable<ICacheable>> GetCacheEntry();
        protected Task RemoveUnusedCacheEntries();
        protected Task<IEnumerable<ICacheable>> AddCacheEntry();
        protected Task<IEnumerable<ICacheable>> RemoveCacheEntry();
    }
}
