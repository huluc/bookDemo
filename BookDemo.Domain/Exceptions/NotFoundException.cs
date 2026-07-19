using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Domain.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string message) : base(message) { }
    }

    public sealed class BookNotFoundException : NotFoundException
    {
        public BookNotFoundException(int id) 
            : base($"Book with id {id} was not found.") { }
    }
    public sealed class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(int id)
            : base($"Category with id {id} was not found.") { }
    }
}
