using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    // NOTE:
    // This abstraction currently exposes repositories managed centrally.
    // Future improvement: resolve repositories directly via DI container
    // instead of manual creation inside RepositoryManager.

    /// <summary>
    /// Acts as a single entry point to access repositories and commit changes.
    /// 
    /// This abstraction represents the Unit of Work pattern:
    /// - Repositories handle data access for specific entities.
    /// - RepositoryManager coordinates those repositories.
    /// - SaveAsync commits all changes as a single transaction.
    ///
    /// Application layer depends only on this abstraction,
    /// not on EF Core or Infrastructure-specific details.
    /// </summary>
    public interface IRepositoryManager
    {
        /// <summary>
        /// Provides access to book-related data operations.
        /// Repository exposes CRUD and query logic,
        /// but does NOT perform persistence commits.
        /// </summary>
        IBookRepository Books { get; }

        /// <summary>
        /// Commits all changes made through repositories
        /// in a single transaction.
        /// </summary>
        void Save();
    }
}
