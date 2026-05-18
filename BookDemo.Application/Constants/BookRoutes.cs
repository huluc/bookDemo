using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Constants
{

    /// <summary>
    /// Contains route names for the Books API endpoints.
    /// Used to avoid magic strings when generating HATEOAS links.
    /// </summary>
    public static class BookRoutes
    {
        public const string GetAll = "GetBooks";
        public const string GetById = "GetBookById";
        public const string Update = "UpdateBook";
        public const string Delete = "DeleteBook";
        public const string Create = "CreateBook";
        public const string Patch = "PatchBook";
    }
}
