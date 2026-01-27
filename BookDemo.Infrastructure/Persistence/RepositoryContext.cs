using BookDemo.Infrastructure.Persistence.Configurations;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace BookDemo.Infrastructure.Persistence
{
    /// <summary>
    /// RepositoryContext represents the EF Core DbContext of the application.
    /// 
    /// Responsibilities:
    /// - Manages the database connection and EF Core session
    /// - Maps domain entities to database tables
    /// - Tracks entity changes (Added, Modified, Deleted)
    /// - Generates SQL queries via LINQ
    /// - Applies Fluent API configurations
    /// - Serves as the persistence infrastructure used by repositories
    /// 
    /// This class is NOT a repository.
    /// Repositories use this DbContext to perform data access operations.
    /// 
    /// Located in the Infrastructure/Persistence layer to isolate
    /// database and EF Core concerns from Application and Domain layers.
    /// </summary>
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
            modelBuilder.ApplyConfiguration(new BookConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
