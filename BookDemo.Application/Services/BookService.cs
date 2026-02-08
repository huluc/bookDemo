using BookDemo.Application.Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Services
{

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

        /// <summary>
        /// IRepositoryManager is injected from DI container.
        /// This gives access to repositories + Save() (Unit of Work).
        /// </summary>
        public BookService(IRepositoryManager manager)
        {
            _manager = manager;
        }

        public IEnumerable<Book> GetBooks()
        {
            // Read-only operation → tracking disabled
            // Improves performance and avoids unnecessary change tracking
            return _manager.Books.GetAll(false);
        }

        public Book? GetBookById(int id)
        {
            // Read-only operation → tracking disabled
            // Returns null if not found (controller decides how to respond)
            return _manager.Books.GetById(id, false);
        }
        public Book CreateBook(Book book)
        {
            if (book is null)
                throw new ArgumentNullException(nameof(book));

            // Repository-level add
            _manager.Books.Add(book);

            // Unit of Work commit
            _manager.Save();

            // Return created entity (Id is now populated)
            return book;

        }
        public bool UpdateBook(int id, Book book)
        {
            // ID mismatch is a business rule violation
            if (book is null || id != book.Id)
                throw new ArgumentException("Book ID mismatch");

            // Tracking ENABLED because we intend to modify the entity
            var existingBook = _manager.Books.GetById(id, trackChanges: true);

            if (existingBook is null)
                return false;

            // Update only allowed fields
            // (this prevents over-posting)
            existingBook.Title = book.Title;
            existingBook.Price = book.Price;

            // EF Core tracks changes automatically
            _manager.Save();

            return true;
        }

        public bool DeleteBook(int id)
        {
            // Tracking ENABLED because entity state will change to Deleted
            var book = _manager.Books.GetById(id, trackChanges: true);

            if (book is null)
                return false;

            _manager.Books.Delete(book);
            _manager.Save();

            return true;
        }


    }
}
