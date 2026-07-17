using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Services.V1;
using BookDemo.Domain.Entities;
using BookDemo.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace BookDemo.Application.Services
{
    public abstract class BookServiceBase : IBookServiceBase
    {
        protected readonly IRepositoryManager _manager;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;
        protected readonly IBookCache _bookCache;

        protected BookServiceBase(IRepositoryManager manager, ILogger logger, IMapper mapper, IBookCache bookCache)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
            _bookCache = bookCache;
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await GetBookOrThrowAsync(id, false);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateBookAsync(BookForCreationDto bookDto)
        {
            if (bookDto is null)
            {
                _logger.LogWarning("CreateBook was called with null book");
                throw new ArgumentNullException(nameof(bookDto));
            }

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("CreateBook payload {@Book}", bookDto);

            _logger.LogInformation("Creating new book. Title={Title}, Price={Price}", bookDto.Title, bookDto.Price);

            var book = _mapper.Map<Book>(bookDto);
            _manager.Books.Add(book);
            await _manager.SaveAsync();
            
            await _bookCache.InvalidateAsync();

            _logger.LogInformation("Book created successfully. Id={BookId}", book.Id);
            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateBookAsync(int id, BookForUpdateDto bookDto)
        {
            if (bookDto is null)
            {
                _logger.LogWarning("Book update failed because payload is null. RouteId={RouteId}", id);
                throw new BadRequestException("Book update payload is null.");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("UpdateBook payload {@Book}", bookDto);

            var existingBook = await GetBookOrThrowAsync(id, trackChanges: true);
            _mapper.Map(bookDto, existingBook);
            await _manager.SaveAsync();
            await _bookCache.InvalidateAsync();

            _logger.LogInformation("Book updated successfully. Id={BookId}", id);
        }

        public async Task DeleteBookAsync(int id)
        {
            _logger.LogDebug("Attempting to delete book. Id={BookId}", id);
            var book = await GetBookOrThrowAsync(id, trackChanges: true);
            _manager.Books.Delete(book);
            await _manager.SaveAsync();
            await _bookCache.InvalidateAsync();
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
            await _bookCache.InvalidateAsync();
        }

        // Shared helper — throws BookNotFoundException if book does not exist.
        protected async Task<Book> GetBookOrThrowAsync(int id, bool trackChanges)
        {
            _logger.LogDebug("Fetching book by Id={BookId}", id);
            var book = await _manager.Books.GetByIdAsync(id, trackChanges);
            if (book is null)
            {
                _logger.LogWarning("Book not found. Id={BookId}", id);
                throw new BookNotFoundException(id);
            }
            _logger.LogInformation("Book fetched successfully. Id={BookId}", id);
            return book;
        }
    }
}