using BookDemo.Application.Contracts;
using BookDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookDemo.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly RepositoryContext _context;
        protected DbSet<T> Set => _context.Set<T>();
        public RepositoryBase(RepositoryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IReadOnlyList<T> GetAll() => Set.AsNoTracking().ToList();


        // -------------------------
        // READ (default: NoTracking)
        // -------------------------

        /// <summary>
        /// Returns items matching the predicate.
        /// Default policy: NoTracking for performance (read-only query).
        /// </summary>
        public IReadOnlyList<T> GetByCondition(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return Set
                .AsNoTracking()
                .Where(predicate)
                .ToList();
        }



        // -------------------------
        // WRITE
        // -------------------------

        public void Add(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            Set.Add(entity);
        }

        public void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            Set.Remove(entity);
        }

        /// <summary>
        /// Marks the given entity for update (detached update).
        ///
        /// WARNING:
        /// EF will treat all scalar properties as modified by default (Update()).
        /// This is fine for PUT-style "replace" updates.
        /// For PATCH (partial update) prefer: load tracked entity -> change selected fields -> SaveChanges.
        /// </summary>
        public void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            Set.Update(entity);
        }


    }
}
