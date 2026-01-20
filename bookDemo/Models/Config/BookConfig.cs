using Microsoft.EntityFrameworkCore;

namespace bookDemo.Models.config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
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
