using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using BookDemo.Domain.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BookDemo.Application.Services
{
    //TODO:
    //✔ NLog + structured logging
    //✔ CorrelationId
    //✔ Request logging
    //✔ UserId enrichment
    //✔ EF Core SQL logging

    //kurulumunu gerçek production standardında adım adım yapalım.

    /// <summary>
    /// BookService contains business logic for Book operations.
    ///
    /// ✔ Coordinates repositories
    /// ✔ Controls tracking behavior
    /// ✔ Commits changes via Unit of Work (Save)
    ///
    /// ❌ Does NOT deal with HTTP concepts (StatusCodes, IActionResult)
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IRepositoryManager _manager;
        private readonly ILogger<BookService> _logger;
        private readonly IMapper _mapper;
        private readonly IBookLinks<BookDto> _bookLinks;
        private readonly IBookLinks<BookDtoV2> _bookLinksV2;

        /// <summary>
        /// IRepositoryManager is injected from DI container.
        /// This gives access to repositories + Save() (Unit of Work).
        /// </summary>
        public BookService(IRepositoryManager manager, ILogger<BookService> logger, IMapper mapper, IBookLinks<BookDto> bookLinks, IBookLinks<BookDtoV2> bookLinksV2)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
            _bookLinks = bookLinks;
            _bookLinksV2 = bookLinksV2;
        }

        public async Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters)
        {
            _logger.LogDebug("Fetching books page {PageNumber} with page size {PageSize} (tracking disabled)",
    parameters.BookQueryParameters.PageNumber, parameters.BookQueryParameters.PageSize);

            // Read-only operation → tracking disabled
            // Improves performance and avoids unnecessary change tracking
            var pagedList = await _manager.Books.GetBooksAsync(parameters.BookQueryParameters, trackChanges: false);
            var bookDtos = _mapper.Map<List<BookDto>>(pagedList);

            var linkedBooks = _bookLinks.TryGenerateLinks(bookDtos, parameters);

            return (linkResponse: linkedBooks, metaData: pagedList.MetaData);
        }

        /// <summary>
        /// V2 version of GetBooksAsync.
        /// Maps to BookDtoV2 which includes the Author field introduced in V2.
        /// </summary>
        public async Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksV2Async(LinkParameters parameters)
        {
            _logger.LogDebug("Fetching books (V2) page {PageNumber} with page size {PageSize} (tracking disabled)",
         parameters.BookQueryParameters.PageNumber, parameters.BookQueryParameters.PageSize);

            // Read-only operation → tracking disabled
            var pagedList = await _manager.Books.GetBooksAsync(parameters.BookQueryParameters, trackChanges: false);

            // V2 — maps to BookDtoV2 which includes Author field
            var bookDtos = _mapper.Map<List<BookDtoV2>>(pagedList);

            var linkedBooks = _bookLinksV2.TryGenerateLinks(bookDtos, parameters);

            return (linkResponse: linkedBooks, metaData: pagedList.MetaData);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            // Read-only operation → tracking disabled
            // Throws BookNotFoundException if the book does not exist.
            // The global exception handler converts it to HTTP 404.
            var book = await GetBookOrThrowAsync(id, false);

            var bookDto = _mapper.Map<BookDto>(book);
            return bookDto;
        }
        public async Task<BookDto> CreateBookAsync(BookForCreationDto bookDto)
        {
            if (bookDto is null)
            {
                _logger.LogWarning("CreateBook was called with null book");
                throw new ArgumentNullException(nameof(bookDto));
            }

            // Debug: gelen payload'u görmek (state inspection)
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("CreateBook payload {@Book}", bookDto);

            _logger.LogInformation("Creating new book. Title={Title}, Price={Price}",
           bookDto.Title, bookDto.Price);

            var book = _mapper.Map<Book>(bookDto);
            // Repository-level add
            _manager.Books.Add(book);

            // Unit of Work commit
            await _manager.SaveAsync();

            _logger.LogInformation("Book created successfully. Id={BookId}", book.Id);

            // Return created entity (Id is now populated)
            return _mapper.Map<BookDto>(book);

        }
        public async Task UpdateBookAsync(int id, BookForUpdateDto bookDto)
        {

            // ID mismatch is a business rule violation
            if (bookDto is null)
            {
                _logger.LogWarning("Book update failed because payload is null. RouteId={RouteId}",
                    id);

                throw new BadRequestException("Book update payload is null.");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("UpdateBook payload {@Book}", bookDto);


            // Tracking ENABLED because we intend to modify the entity
            var existingBook = await GetBookOrThrowAsync(id, trackChanges: true);

            // Update only the incoming fields
            //if(book.Title is not null)
            //existingBook.Title = book.Title;

            //if (book.Price.HasValue)
            //    existingBook.Price = book.Price.Value;

            _mapper.Map(bookDto, existingBook);

            // EF Core tracks changes automatically
            await _manager.SaveAsync();

            _logger.LogInformation("Book updated successfully. Id={BookId}", id);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Book after update {@Book}", existingBook);

        }

        public async Task DeleteBookAsync(int id)
        {
            _logger.LogDebug("Attempting to delete book. Id={BookId}", id);
            // Tracking ENABLED because entity state will change to Deleted
            var book = await GetBookOrThrowAsync(id, trackChanges: true);

            _manager.Books.Delete(book);
            await _manager.SaveAsync();

            _logger.LogInformation("Book deleted successfully. Id={BookId}", id);
        }

        public async Task<(BookForUpdateDto bookToPatch, Book bookEntity)> GetBookForPatchAsync(int id)
        {
            var bookEntity = await GetBookOrThrowAsync(id, trackChanges: true);

            var bookToPatch = _mapper.Map<BookForUpdateDto>(bookEntity);

            return (bookToPatch, bookEntity);
        }

        public async Task SaveChangesForPatchAsync(BookForUpdateDto bookToPatch, Book bookEntity)
        {
            _mapper.Map(bookToPatch, bookEntity);
            await _manager.SaveAsync();
        }
        private async Task<Book> GetBookOrThrowAsync(int id, bool trackChanges)
        {
            _logger.LogDebug("Fetching book by Id={BookId}", id);
            var book = await _manager.Books.GetByIdAsync(id, trackChanges);
            if (book is null)
            {
                _logger.LogWarning("Book not found. Id={BookId}", id);
                throw new BookNotFoundException(id);
            }
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Book found {@Book}", book);

            _logger.LogInformation("Book fetched successfully. Id={BookId}", id);
            return book;
        }

    }
}
