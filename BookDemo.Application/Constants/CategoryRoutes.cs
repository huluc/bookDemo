using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Constants
{
    /// <summary>
    /// Contains route names for the Categories API endpoints.
    /// Used to avoid magic strings when generating HATEOAS links.
    /// </summary>
    public static class CategoryRoutes
    {
        public const string GetAll = "GetCategories";
        public const string GetById = "GetCategoryById";
        public const string GetBooksByCategory = "GetBooksByCategory";
        public const string Create = "CreateCategory";
        public const string Update = "UpdateCategory";
        public const string Delete = "DeleteCategory";
    }
}
