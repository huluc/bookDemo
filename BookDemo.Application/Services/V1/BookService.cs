using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.Contracts.V1;
using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using Entities.Models;
using Microsoft.Extensions.Logging;
using IBookService = BookDemo.Application.Contracts.V1.IBookService;

namespace BookDemo.Application.Services.V1
{
    /// <summary>
    /// V1 BookService — returns books without Author field.
    /// Common operations are inherited from BookServiceBase.
    /// </summary>
    public class BookService : BookServiceBase, IBookService
    {
        private readonly IBookLinks<BookDto> _bookLinks;

        public BookService(IRepositoryManager manager, ILogger<BookService> logger, IMapper mapper, IBookLinks<BookDto> bookLinks, IBookCache bookCache)
            : base(manager, logger, mapper, bookCache)
        {
            _bookLinks = bookLinks;
        }

        public async Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters)
        {
            _logger.LogDebug("Fetching books page {PageNumber} with page size {PageSize} (tracking disabled)",
                parameters.BookQueryParameters.PageNumber, parameters.BookQueryParameters.PageSize);

            // Read-only operation → tracking disabled
            var pagedList = await _manager.Books.GetBooksAsync(parameters.BookQueryParameters, trackChanges: false);
            var bookDtos = _mapper.Map<List<BookDto>>(pagedList);
            var linkedBooks = _bookLinks.TryGenerateLinks(bookDtos, parameters);

            return (linkResponse: linkedBooks, metaData: pagedList.MetaData);
        }
    }
}