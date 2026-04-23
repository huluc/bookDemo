using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Repositories.Extensions
{
    public static class BookRepositoryExtensions
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, decimal? minPrice, decimal? maxPrice)
        {
            if (minPrice.HasValue)
                books = books.Where(b => b.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                books = books.Where(b => b.Price <= maxPrice.Value);
            return books;
        }

        public static IQueryable<Book> Search(this IQueryable<Book> books, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return books;

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return books.Where(b =>
                   b.Title.ToLower().Contains(lowerCaseTerm));
        }
    }
}
