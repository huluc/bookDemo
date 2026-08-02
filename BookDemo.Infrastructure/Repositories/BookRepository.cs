                             using BookDemo.Application.Contracts;
using BookDemo.Application.RequestFeatures;
using BookDemo.Domain.Entities;
using BookDemo.Infrastructure.Persistence;
using BookDemo.Infrastructure.Repositories.Extensions;
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
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
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
                // Note: We use FirstOrDefaultAsync instead of FindAsync here.
                // FindAsync does NOT return an IQueryable<T> - it first checks the
                // context's change tracker for an already-tracked entity with this key,
                // and only queries the database if it isn't found. Because it doesn't
                // produce a composable query, you cannot chain .Include()/.ThenInclude()
                // onto it. Also, if the entity happened to already be tracked, FindAsync
                // would skip the DB call entirely and the related data (BookCategories,
                // Category) would never be loaded - an inconsistent result depending on
                // tracking state. FirstOrDefaultAsync avoids this by always running an
                // explicit, composable query.
                return await Set
                    .Include(b => b.BookCategories)
                        .ThenInclude(bc => bc.Category)
                    .FirstOrDefaultAsync(b => b.Id == id);
            else
                // AsNoTracking + SingleOrDefaultAsync: read-only scenario, no change
                // tracking overhead. SingleOrDefaultAsync also works fine here since
                // Id is unique and we expect at most one match.
                return await Set
                    .AsNoTracking()
                    .Include(b => b.BookCategories)
                        .ThenInclude(bc => bc.Category)
                    .SingleOrDefaultAsync(b => b.Id == id);
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
