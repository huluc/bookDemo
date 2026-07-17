using BookDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// CategoryConfiguration defines EF Core Fluent API
    /// mappings for the Category entity.
    /// 
    /// Responsibilities:
    /// - Configures table name, keys, and column constraints
    /// - Seeds initial category data
    /// - Keeps entity classes clean from persistence concerns
    /// 
    /// This configuration is applied by RepositoryContext
    /// during model creation.
    /// </summary>
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasData(
                new Category { Id = 1, Name = "Fiction" },
                new Category { Id = 2, Name = "Non-Fiction" },
                new Category { Id = 3, Name = "Science" },
                new Category { Id = 4, Name = "History" }
            );
        }
    }
}
