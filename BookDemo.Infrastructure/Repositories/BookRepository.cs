using BookDemo.Application.Contracts;
using BookDemo.Infrastructure.Persistence;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Repositories
{
    public sealed class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext context) : base(context)
        {
        }

        public bool Exists(int id)
        {
            return Set.AsNoTracking().Any(b => b.Id == id);
        }

        public Book? GetById(int id)
        {
            return Set.AsNoTracking().SingleOrDefault(b => b.Id == id);
        }

        public IReadOnlyList<Book> GetByTitleContains(string text)
        {
            throw new NotImplementedException(); //TODO: Implement this method
        }
    }
}
