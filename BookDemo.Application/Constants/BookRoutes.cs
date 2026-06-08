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
        // V1 routes
        public const string GetAll = "GetBooks";
        public const string GetById = "GetBookById";
        public const string Update = "UpdateBook";
        public const string Delete = "DeleteBook";
        public const string Create = "CreateBook";
        public const string Patch = "PatchBook";

        // V2 routes
        public const string GetAllV2 = "GetBooksV2";
        public const string GetByIdV2 = "GetBookByIdV2";
        public const string UpdateV2 = "UpdateBookV2";
        public const string DeleteV2 = "DeleteBookV2";
        public const string CreateV2 = "CreateBookV2";
        public const string PatchV2 = "PatchBookV2";
    }
}
