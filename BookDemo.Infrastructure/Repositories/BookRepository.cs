using BookDemo.Application.Contracts;
using BookDemo.Application.RequestFeatures;
using BookDemo.Infrastructure.Persistence;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Repositories
{
    public sealed class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await Set.AsNoTracking().AnyAsync(b => b.Id == id);
        }

        public async Task<IReadOnlyList<Book>> GetBooksAsync(BookQueryParameters parameters, bool trackChanges)
        {
            IQueryable<Book> query = trackChanges ? Set : Set.AsNoTracking();

           query = query
                .OrderBy(b => b.Id)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize);

            return await query.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id, bool trackChanges)
        {
            if (trackChanges)
                return await Set.FindAsync(id);
            else
                return await Set.AsNoTracking().SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IReadOnlyList<Book>> GetByTitleContainsAsync(string text)
        {
            return await Set.
                AsNoTracking()
                .Where(b => b.Title.Contains(text))
                .ToListAsync();
        }

    }
}
