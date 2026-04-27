using BookDemo.Application.Contracts;
using BookDemo.Application.RequestFeatures;
using BookDemo.Infrastructure.Persistence;
using BookDemo.Infrastructure.Repositories.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedList<Book>> GetBooksAsync(BookQueryParameters parameters, bool trackChanges)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            IQueryable<Book> query = trackChanges ? Set : Set.AsNoTracking();

            query = query
                .FilterBooks(parameters.MinPrice, parameters.MaxPrice)
                .Search(parameters.SearchTerm)
                .Sort(parameters.OrderBy);


            var count = await query.CountAsync();

            var books = await query
                 .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                 .Take(parameters.PageSize)
                 .ToListAsync();

            return new PagedList<Book>(books, count, parameters.PageNumber, parameters.PageSize);
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
