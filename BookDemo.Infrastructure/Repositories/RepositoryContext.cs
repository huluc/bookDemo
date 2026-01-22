using bookDemo.Repositories;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace bookDemo.Infrastructure.Repositories
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
            modelBuilder.ApplyConfiguration(new BookConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
