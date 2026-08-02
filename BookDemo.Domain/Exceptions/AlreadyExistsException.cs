using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Domain.Exceptions
{
    public abstract class AlreadyExistsException : Exception
    {
        protected AlreadyExistsException(string message) :  base(message) { }
    }
    public sealed class BookAlreadyExistsException : AlreadyExistsException
    {
        public BookAlreadyExistsException(string title)
            : base($"Book with title '{title}' already exists.") { }
    }
    public sealed class CategoryAlreadyExistsException : AlreadyExistsException
    {
        public CategoryAlreadyExistsException(string name)
            : base($"Category with name '{name}' already exists.") { }
    }
    public sealed class BookCategoryAlreadyExistsException : AlreadyExistsException
    {
        public BookCategoryAlreadyExistsException(int bookId, int categoryId)
            : base($"Book with id {bookId} is already assigned to category with id {categoryId}.") { }
    }
}
