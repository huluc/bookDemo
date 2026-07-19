using BookDemo.Application.Contracts;
using BookDemo.Infrastructure.Persistence;
using Microsoft.Identity.Client.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        /// <summary>
        /// Acts as a central access point for all repositories.
        /// This class also represents the Unit of Work pattern,
        /// coordinating repository access and SaveChanges.
        /// </summary>
        // TODO (Refactor):
        // Repositories are instantiated manually here to avoid registering
        // each repository separately in the IoC container.
        // This is a conscious design choice for simplicity.
        //
        // Consider refactoring to full DI-based repository resolution
        // (e.g., constructor injection or assembly scanning)
        // if the number of repositories grows or additional dependencies
        // are introduced in repositories.

        // Holds the shared DbContext instance.
        // All repositories created by this manager
        // will use the same context instance.
        private readonly RepositoryContext _context;

        // Lazy initialization ensures that the BookRepository
        // is created only when it is first accessed.
        // This avoids unnecessary object creation.
        private readonly Lazy<IBookRepository> _bookRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;

        /// <summary>
        /// Initializes the RepositoryManager with a DbContext.
        /// The DbContext is injected via Dependency Injection.
        /// </summary>
        /// <param name="context">The EF Core DbContext instance</param>
        public RepositoryManager(RepositoryContext context)
        {
            // Defensive programming:
            // Ensure the injected context is not null.
            _context = context ?? throw new ArgumentNullException(nameof(context));

            // Configure lazy creation of BookRepository.
            // The repository will be instantiated only
            // when the Value property is accessed.
            // This line does NOT create the repository instance.
            // It only stores a factory that knows how to create it later.
            // The repository will be created using the shared DbContext (_context)
            // when it is first requested.
            _bookRepository = new Lazy<IBookRepository>(() => new BookRepository(_context));
            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));

        }

        // Accessing Value triggers the creation of the repository
        // on the first call.
        // Subsequent calls return the same already-created instance.
        public IBookRepository Books => _bookRepository.Value;
        public ICategoryRepository Categories => _categoryRepository.Value;


        /// <summary>
        /// Persists all changes made through repositories
        /// to the database in a single transaction.
        /// </summary>
        public async Task SaveAsync() => await _context.SaveChangesAsync();

    }
}
