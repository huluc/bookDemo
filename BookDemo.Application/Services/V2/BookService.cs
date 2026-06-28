using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using Entities.Models;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using IBookServiceV2 = BookDemo.Application.Contracts.V2.IBookService;

namespace BookDemo.Application.Services.V2
{
    /// <summary>
    /// V2 BookService — returns books with Author field.
    /// Common operations are inherited from BookServiceBase.
    /// </summary>
    public class BookService : BookServiceBase, IBookServiceV2
    {
        private readonly IBookLinks<BookDtoV2> _bookLinks;
        private readonly HybridCache _cache;
        public BookService(IRepositoryManager manager, ILogger<BookService> logger, IMapper mapper, IBookLinks<BookDtoV2> bookLinks, HybridCache cache)
            : base(manager, logger, mapper)
        {
            _bookLinks = bookLinks;
            _cache = cache;
        }

        public async Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters)
        {
            var p = parameters.BookQueryParameters;

            // Unique cache key per query combination — prevents returning wrong cached data
            // for different pages, filters, or sort orders.
            var cacheKey = $"books:v2:page:{p.PageNumber}:size:{p.PageSize}:search:{p.SearchTerm}:order:{p.OrderBy}:minPrice:{p.MinPrice}:maxPrice:{p.MaxPrice}";

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    // This factory runs only on cache miss — i.e., when the data is not
                    // in L1 (in-process memory) or L2 (distributed cache).
                    // On cache hit, this block is skipped entirely and no DB query is made.
                    _logger.LogDebug("Cache miss — fetching books (V2) page {PageNumber} size {PageSize} from DB",
                        p.PageNumber, p.PageSize);

                    // Read-only operation → tracking disabled for better performance.
                    // Maps to BookDtoV2 which includes the Author field (V1 does not).
                    var pagedList = await _manager.Books.GetBooksAsync(p, trackChanges: false);
                    var bookDtos = _mapper.Map<List<BookDtoV2>>(pagedList);
                    var linkedBooks = _bookLinks.TryGenerateLinks(bookDtos, parameters);

                    return (linkResponse: linkedBooks, metaData: pagedList.MetaData);
                },
                new HybridCacheEntryOptions
                {
                    // L2 (distributed cache / Redis) expiration — how long the data lives in shared cache.
                    Expiration = TimeSpan.FromMinutes(2),
                    // L1 (in-process memory) expiration — must be <= Expiration.
                    // Shorter than L2 to reduce stale data risk on the current server.
                    LocalCacheExpiration = TimeSpan.FromMinutes(1),
                },
                // Tag-based invalidation — all cache entries tagged "books" can be
                // removed at once via RemoveByTagAsync("books") on write operations.
                tags: new[] { "books" }
            );
        }
    }
}