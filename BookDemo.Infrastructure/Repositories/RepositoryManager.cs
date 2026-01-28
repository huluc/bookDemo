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
        // TODO (Refactor):
        // Repositories are instantiated manually here to avoid registering
        // each repository separately in the IoC container.
        // This is a conscious design choice for simplicity.
        //
        // Consider refactoring to full DI-based repository resolution
        // (e.g., constructor injection or assembly scanning)
        // if the number of repositories grows or additional dependencies
        // are introduced in repositories.
        private readonly RepositoryContext _context;
        public RepositoryManager(RepositoryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IBookRepository Books => new BookRepository(_context);

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
