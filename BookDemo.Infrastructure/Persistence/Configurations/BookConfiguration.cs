using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace BookDemo.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// BookConfiguration defines EF Core Fluent API
    /// mappings for the Book entity.
    /// 
    /// Responsibilities:
    /// - Configures table name, keys, and relationships
    /// - Defines column constraints and precision
    /// - Keeps entity classes clean from persistence concerns
    /// 
    /// This configuration is applied by RepositoryContext
    /// during model creation.
    /// </summary>
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Book> builder)
        {
            builder.HasData(
            new Book
            {
                Id = 1,
                Title = "Clean Code",
                Price = 45.90m
            },
             new Book
             {
                 Id = 2,
                 Title = "Domain-Driven Design",
                 Price = 59.99m
             },
             new Book
             {
                 Id = 3,
                 Title = "The Pragmatic Programmer",
                 Price = 39.95m
             }
     );
        }
    }
}
