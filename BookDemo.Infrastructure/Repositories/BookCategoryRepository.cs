using BookDemo.Application.Contracts;
using BookDemo.Domain.Entities;
using BookDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Repositories
{
    public sealed class BookCategoryRepository : RepositoryBase<BookCategory>, IBookCategoryRepository
    {
        public BookCategoryRepository(RepositoryContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(int bookId, int categoryId)
        {
            return Set.AnyAsync(bc => bc.BookId == bookId && bc.CategoryId == categoryId);
        }

        public async Task<BookCategory?> GetAsync(int bookId, int categoryId, bool trackChanges)
        {
            var query = trackChanges ? Set : Set.AsNoTracking();

            return await query.SingleOrDefaultAsync(bc =>
                bc.BookId == bookId && bc.CategoryId == categoryId);
        }
        public async Task<IReadOnlyList<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await Set
                .AsNoTracking()
                .Where(bc => bc.CategoryId == categoryId)
                .Select(bc => bc.Book)
                .ToListAsync();
        }
    }
}
