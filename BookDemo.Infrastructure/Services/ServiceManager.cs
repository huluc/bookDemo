using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Services
{
    /// <summary>
    /// Concrete implementation of IServiceManager.
    ///
    /// - Acts as a facade over all application services
    /// - Creates services lazily (on first access)
    /// - Shares common dependencies between services
    ///   (e.g. IRepositoryManager)
    /// </summary>
    /// /// <summary>
    /// Sealed to prevent inheritance.
    /// This class is a simple facade over services and is not intended
    /// to be extended via subclassing (composition is preferred).
    /// </summary>
    public sealed class ServiceManager : IServiceManager
    {
        // Lazy initialization ensures that the service
        // is created only when it is actually requested.
        private readonly Lazy<IBookService> _bookService;
        /// <summary>
        /// Initializes the ServiceManager with required dependencies.
        ///
        /// Usually depends on IRepositoryManager, because
        /// services coordinate repositories to perform business logic.
        /// </summary>
        public ServiceManager(IRepositoryManager repositoryManager, ILogger<BookService> loggerService, IMapper mapper, IBookLinks<BookDto> bookLinks, IBookLinks<BookDtoV2> bookLinksV2)
        {
            if (repositoryManager is null)
                throw new ArgumentNullException(nameof(repositoryManager));

            // BookService is created lazily and receives
            // the shared repository manager.
            _bookService = new Lazy<IBookService>(
                () => new BookService(repositoryManager, loggerService, mapper, bookLinks, bookLinksV2)
            );
        }
        /// <summary>
        /// Provides access to the BookService.
        /// The service instance is created on first access.
        /// </summary>
        /// 
        public IBookService BookService => _bookService.Value;
    }
}
