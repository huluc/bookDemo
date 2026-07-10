using BookDemo.Infrastructure.Identity;
using BookDemo.Infrastructure.Persistence.Configurations;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    /// - Inherits Identity's user/role/claim tables via IdentityDbContext
    /// 
    /// This class is NOT a repository.
    /// Repositories use this DbContext to perform data access operations.
    /// 
    /// Located in the Infrastructure/Persistence layer to isolate
    /// database and EF Core concerns from Application and Domain layers.
    /// </summary>
    // DbContext -> IdentityDbContext<ApplicationUser> change:
    // Tells EF Core "also add Identity's tables to this context".
    // IdentityDbContext<TUser> derives from DbContext under the hood and
    // automatically defines these tables: AspNetUsers, AspNetRoles,
    // AspNetUserRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserTokens,
    // AspNetRoleClaims. No need to write these tables by hand, they come for free.

    public class RepositoryContext : IdentityDbContext<ApplicationUser>
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating() call order changed: now called FIRST.
            // Previously it was called last. When using Identity, calling this
            // first is the more correct practice: IdentityDbContext's own
            // OnModelCreating override defines the rules for Identity's tables
            // (primary keys, indexes, relationships). Running it first, then
            // applying our own BookConfiguration on top, guarantees that our
            // configuration takes priority in case of any potential conflict.
            // In practice there's no conflict between Book and Identity tables,
            // but this is the generally accepted ordering: "set up the
            // framework's foundation first, then add your own customization".
            base.OnModelCreating(modelBuilder);

            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
            modelBuilder.ApplyConfiguration(new BookConfiguration());
        }
    }
}