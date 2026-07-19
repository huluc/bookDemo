using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    /// <summary>
    /// Provides a single entry point to the service layer.
    ///
    /// Controllers can access multiple domain services
    /// through this interface instead of injecting each
    /// service individually.
    ///
    /// Think of this as the service-layer counterpart
    /// of IRepositoryManager.
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// V1 book service — books without Author field.
        /// </summary>
        V1.IBookService BookService { get; }

        /// <summary>
        /// V2 book service — books with Author field.
        /// </summary>
        V2.IBookService BookServiceV2 { get; }
        ICategoryService CategoryService { get; }

    }
}
