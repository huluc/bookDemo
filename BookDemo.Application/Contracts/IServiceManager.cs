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
        /// Exposes book-related business operations.
        /// </summary>
        IBookService BookService { get; }
    }
}
