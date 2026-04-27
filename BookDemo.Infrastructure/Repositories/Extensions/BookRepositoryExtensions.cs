using Entities.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;

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

        public static IQueryable<Book> Sort(this IQueryable<Book> books, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return books.OrderBy(b => b.Id);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return books.OrderBy(b => b.Id);

            return books.OrderBy(orderQuery);
        }
    }
}