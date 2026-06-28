using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using Entities.Models;
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

        public BookService(IRepositoryManager manager, ILogger<BookService> logger, IMapper mapper, IBookLinks<BookDtoV2> bookLinks)
            : base(manager, logger, mapper)
        {
            _bookLinks = bookLinks;
        }

        public async Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters)
        {
            _logger.LogDebug("Fetching books (V2) page {PageNumber} with page size {PageSize} (tracking disabled)",
                parameters.BookQueryParameters.PageNumber, parameters.BookQueryParameters.PageSize);

            // Read-only operation → tracking disabled
            // Maps to BookDtoV2 which includes Author field
            var pagedList = await _manager.Books.GetBooksAsync(parameters.BookQueryParameters, trackChanges: false);
            var bookDtos = _mapper.Map<List<BookDtoV2>>(pagedList);
            var linkedBooks = _bookLinks.TryGenerateLinks(bookDtos, parameters);

            return (linkResponse: linkedBooks, metaData: pagedList.MetaData);
        }
    }
}