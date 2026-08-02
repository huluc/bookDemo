using BookDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    public interface IBookCategoryRepository :IRepositoryBase<BookCategory>
    {
        /// <summary>
        /// Gets a specific Book-Category assignment by its composite key.
        /// Used to check whether a book is already assigned to a category.
        /// </summary>
        Task<BookCategory?> GetAsync(int bookId, int categoryId, bool trackChanges);
        Task<bool> ExistsAsync(int bookId, int categoryId);
        Task<IReadOnlyList<Book>> GetBooksByCategoryAsync(int categoryId);
    }
}
