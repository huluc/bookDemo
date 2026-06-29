using BookDemo.Application.Contracts;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Caching
{
    /// <summary>
    /// Invalidates all "books" tagged cache entries via HybridCache.
    /// Used by both V1 and V2 to ensure cache consistency across API versions.
    /// </summary>
    public class HybridBookCache : IBookCache
    {
        private readonly HybridCache _cache;

        public HybridBookCache(HybridCache cache)
        {
            _cache = cache;
        }

        public async Task InvalidateAsync()
        {
           await _cache.RemoveByTagAsync("books");
        }
    }
}
