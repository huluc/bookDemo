using BookDemo.Application.RequestFeatures;
using Entities.Models;
namespace BookDemo.Application.Contracts
{
    public interface IBookRepository:IRepositoryBase<Book>
    {
        /// <summary>
        /// Gets a single Book by its primary key.
        /// Type-safe alternative to a generic GetById(object id).
        /// </summary>
        Task<Book?> GetByIdAsync(int id, bool trackChanges);

        /// <summary>
        /// Checks if a book exists by id.
        /// Useful for validation without loading the whole entity.
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Optional: domain-specific query example.
        /// Adjust/remove based on your needs.
        /// </summary>
        Task<IReadOnlyList<Book>> GetByTitleContainsAsync(string text);

        Task<PagedList<Book>> GetBooksAsync(BookQueryParameters parameters, bool trackChanges);
    }
}
