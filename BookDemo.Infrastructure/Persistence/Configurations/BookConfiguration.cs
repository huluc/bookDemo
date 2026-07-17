using BookDemo.Domain.Entities;
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
                    Title = "1984",
                    Author = "George Orwell",
                    Price = 15.99m
                },
                new Book
                {
                    Id = 2,
                    Title = "Sapiens: A Brief History of Humankind",
                    Author = "Yuval Noah Harari",
                    Price = 22.50m
                },
                new Book
                {
                    Id = 3,
                    Title = "A Brief History of Time",
                    Author = "Stephen Hawking",
                    Price = 18.75m
                }
            );
        }
    }
}
