using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;

using BookDemo.Domain.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        protected IRepositoryManager _manager;
        private readonly ILogger<BookService> _logger;
        private readonly IMapper _mapper;
        /// <summary>
        /// IRepositoryManager is injected from DI container.
        /// This gives access to repositories + Save() (Unit of Work).
        /// </summary>
        public BookService(IRepositoryManager manager, ILogger<BookService> logger, IMapper mapper)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<BookDto> GetBooks()
        {
            _logger.LogDebug("Fetching all books (tracking disabled)");

            // Read-only operation → tracking disabled
            // Improves performance and avoids unnecessary change tracking
            var books = _manager.Books.GetAll(trackChanges: false);
            var bookDtos = _mapper.Map<List<BookDto>>(books);
            _logger.LogInformation("Fetched {Count} books successfully", bookDtos.Count);

            return bookDtos;
        }

        public Book? GetBookById(int id)
        {
            _logger.LogDebug("Fetching book by Id={BookId}", id);

            // Read-only operation → tracking disabled
            // Throws BookNotFoundException if the book does not exist.
            // The global exception handler converts it to HTTP 404.
            var book = _manager.Books.GetById(id, false);

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
        public Book CreateBook(Book book)
        {
            if (book is null)
            {
                _logger.LogWarning("CreateBook was called with null book");
                throw new ArgumentNullException(nameof(book));
            }

            // Debug: gelen payload'u görmek (state inspection)
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("CreateBook payload {@Book}", book);

            _logger.LogInformation("Creating new book. Title={Title}, Price={Price}",
           book.Title, book.Price);

            // Repository-level add
            _manager.Books.Add(book);

            // Unit of Work commit
            _manager.Save();

            _logger.LogInformation("Book created successfully. Id={BookId}", book.Id);

            // Return created entity (Id is now populated)
            return book;

        }
        public void UpdateBook(int id, BookForUpdateDto bookDto)
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
            var existingBook = _manager.Books.GetById(id, trackChanges: true);

            if (existingBook is null)
            {
                _logger.LogWarning("Update failed. Book not found. Id={BookId}", id);
                throw new BookNotFoundException(id);
            }

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Book before update {@Book}", existingBook);

            // Update only the incoming fields
            //if(book.Title is not null)
            //existingBook.Title = book.Title;

            //if (book.Price.HasValue)
            //    existingBook.Price = book.Price.Value;

            _mapper.Map(bookDto, existingBook);

            // EF Core tracks changes automatically
            _manager.Save();

            _logger.LogInformation("Book updated successfully. Id={BookId}", id);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("Book after update {@Book}", existingBook);

        }

        public bool DeleteBook(int id)
        {
            _logger.LogDebug("Attempting to delete book. Id={BookId}", id);
            // Tracking ENABLED because entity state will change to Deleted
            var book = _manager.Books.GetById(id, trackChanges: true);

            if (book is null)
            {
                _logger.LogWarning("Delete failed. Book not found. Id={BookId}", id);
                throw new BookNotFoundException(id);
            }

            _manager.Books.Delete(book);
            _manager.Save();

            _logger.LogInformation("Book deleted successfully. Id={BookId}", id);

            return true;
        }

        public BookForUpdateDto GetBookForPatch(int id)
        {
            var bookEntity = _manager.Books.GetById(id, trackChanges: false);

            if (bookEntity is null)
                throw new BookNotFoundException(id);

            return _mapper.Map<BookForUpdateDto>(bookEntity);
        }
    }
}
