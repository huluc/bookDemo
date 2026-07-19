using BookDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        /// <summary>
        /// Gets a single Category by its primary key.
        /// Type-safe alternative to a generic GetById(object id).
        /// </summary>
        Task<Category?> GetByIdAsync(int id, bool trackChanges);

        /// <summary>
        /// Checks if a category exists by id.
        /// Useful for validation without loading the whole entity.
        /// </summary>
        Task<bool> ExistsAsync(int id);
    }
}
