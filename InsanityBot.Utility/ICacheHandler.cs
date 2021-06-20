using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsanityBot.Utility
{
	internal interface ICacheHandler<T, U> where T : ICacheable
	{
		public IEnumerable<T> Cache { get; set; }

		public Task<T> GetCacheEntry(U id);
		public Task RemoveUnusedCacheEntries();
		public Task<T> AddCacheEntry(U id);
		public Task RemoveCacheEntry(T entry);
	}
}
