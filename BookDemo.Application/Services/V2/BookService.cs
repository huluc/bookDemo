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

        public BookService(IRepositoryManager manager, ILogger<BookService> logger, IMapper mapper, IBookLinks<BookDtoV2> bookLinks, HybridCache cache, IBookCache bookCache)
            : base(manager, logger, mapper, bookCache)
        {
            _bookLinks = bookLinks;
            _cache = cache;
        }

        public async Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters)
        {
            var p = parameters.BookQueryParameters;

            var cacheKey = $"books:v2:page:{p.PageNumber}:size:{p.PageSize}:search:{p.SearchTerm}:order:{p.OrderBy}:minPrice:{p.MinPrice}:maxPrice:{p.MaxPrice}";

            // Cache only raw data — not the shaped/linked result
            var (bookDtos, metaData) = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    _logger.LogDebug("Cache miss — fetching books (V2) page {PageNumber} size {PageSize} from DB",
                        p.PageNumber, p.PageSize);

                    var pagedList = await _manager.Books.GetBooksAsync(p, trackChanges: false);
                    var dtos = _mapper.Map<List<BookDtoV2>>(pagedList);

                    return (bookDtos: dtos, metaData: pagedList.MetaData);
                },
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(2),
                    LocalCacheExpiration = TimeSpan.FromMinutes(1),
                },
                tags: new[] { "books" }
            );

            // TryGenerateLinks runs after cache — not serialized
            var linkedBooks = _bookLinks.TryGenerateLinks(bookDtos, parameters);

            return (linkResponse: linkedBooks, metaData: metaData);
        }
    }
}