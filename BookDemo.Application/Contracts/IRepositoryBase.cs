using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookDemo.Application.Contracts
{
    /// <summary>
    /// Generic repository contract for the Application layer (synchronous version).
    ///
    /// Purpose:
    /// - Defines WHAT the application needs from persistence operations,
    ///   without exposing HOW those operations are implemented.
    ///
    /// Important:
    /// - No EF Core / DbContext / tracking / IQueryable should appear here.
    /// - Implementations live in the Infrastructure layer (EF Core, Dapper, etc.).
    /// </summary>
    /// <summary>
    /// IRepositoryBase defines a generic contract for basic
    /// data access operations used by the application.
    /// 
    /// Responsibilities:
    /// - Provides common CRUD method signatures
    /// - Abstracts persistence concerns from the Application layer
    /// - Allows infrastructure-specific implementations (e.g., EF Core)
    /// 
    /// Design notes:
    /// - Returning IQueryable exposes query composition and can be considered
    ///   a leaky abstraction. This is a deliberate design choice here,
    ///   but may be refactored later to return concrete collections or DTOs.
    /// - The 'trackChanges' parameter controls EF Core change tracking behavior
    ///   (tracked vs. AsNoTracking queries).
    /// 
    /// This interface contains NO EF Core, database, or infrastructure logic.
    /// Implementations live in the Infrastructure layer.
    /// </summary>

    // Marked as internal to prevent the Application layer from depending on EF Core details.
    // This ensures that EF Core–specific logic stays isolated within the Infrastructure layer.
    public interface IRepositoryBase<T>
    {

        /// <summary>
        /// Returns items matching the predicate.
        /// The implementation decides how to evaluate it (e.g., EF Core translates to SQL).
        /// </summary>
        IReadOnlyList<T> GetByCondition(Expression<Func<T, bool>> predicate, bool trackChanges);

        public IReadOnlyList<T> GetAll(bool trackChanges);

        // WRITE
        void Add(T entity);
        void Delete(T entity);

        /// <summary>
        /// Marks an entity for update. The implementation decides how updates are handled.
        /// </summary>
        void Update(T entity);
    }
}
