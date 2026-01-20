using bookDemo.Models;
using bookDemo.Models.config;
using Microsoft.EntityFrameworkCore;

namespace bookDemo.Repositories
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
