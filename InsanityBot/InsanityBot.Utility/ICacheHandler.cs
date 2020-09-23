using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InsanityBot.Utility
{
    interface ICacheHandler<T> where T : ICacheable
    {
        public IEnumerable<T> Cache { get; protected set; }
        protected Timer CacheIterationTimer { get; set; }

        public Task<IEnumerable<T>> GetCacheEntry();
        protected Task RemoveUnusedCacheEntries();
        protected Task<IEnumerable<T>> AddCacheEntry();
        protected Task<IEnumerable<T>> RemoveCacheEntry();
    }
}
