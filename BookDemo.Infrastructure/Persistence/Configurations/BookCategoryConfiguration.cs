using BookDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// BookCategoryConfiguration defines EF Core Fluent API
    /// mappings for the BookCategory join entity, which represents
    /// the many-to-many relationship between Book and Category.
    /// 
    /// Responsibilities:
    /// - Defines the composite primary key (BookId + CategoryId)
    /// - Configures the two one-to-many relationships that together
    ///   form the many-to-many join (Book -> BookCategory, Category -> BookCategory)
    /// - Seeds initial book-category assignments

    /// This configuration is applied by RepositoryContext
    /// during model creation.
    /// </summary>
    public class BookCategoryConfiguration : IEntityTypeConfiguration<BookCategory>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BookCategory> builder)
        {
            // Composite key: the combination of BookId + CategoryId must be
            // unique. This is what makes it a valid many-to-many join table —
            // the same book cannot be linked to the same category twice.
            builder.HasKey(bc => new { bc.BookId, bc.CategoryId });

            builder.HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(bc => bc.BookId);

            builder.HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(bc => bc.CategoryId);

            // Seed data must use raw FK values only — navigation properties
            // (Book, Category) cannot be set here, since HasData bypasses
            // EF Core's change tracker and writes rows directly.
            builder.HasData(
                // 1984 (1) -> Fiction (1)
                new BookCategory { BookId = 1, CategoryId = 1 },

                // Sapiens (2) -> Non-Fiction (2)
                new BookCategory { BookId = 2, CategoryId = 2 },

                // Sapiens (2) -> also History (4), demonstrating the
                // many-to-many nature: one book can belong to more than
                // one category.
                new BookCategory { BookId = 2, CategoryId = 4 },

                // A Brief History of Time (3) -> Science (3)
                new BookCategory { BookId = 3, CategoryId = 3 },

                // A Brief History of Time (3) -> also Non-Fiction (2)
                new BookCategory { BookId = 3, CategoryId = 2 }
            );

        }
    }
}
