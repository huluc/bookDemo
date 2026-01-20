using bookDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace bookDemo.Repositories
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
    }
}
