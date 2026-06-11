using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using Microsoft.Extensions.Logging;
using V1BookService = BookDemo.Application.Services.V1.BookService;
using V2BookService = BookDemo.Application.Services.V2.BookService;
using IBookServiceV1 = BookDemo.Application.Contracts.V1.IBookService;
using IBookServiceV2 = BookDemo.Application.Contracts.V2.IBookService;

namespace BookDemo.Infrastructure.Services
{
    /// <summary>
    /// ServiceManager is the single entry point to the application's service layer.
    ///
    /// It acts as a facade — controllers only need to inject IServiceManager
    /// instead of each individual service separately.
    ///
    /// Services are initialized lazily: a service instance is only created
    /// when it is first accessed, not when ServiceManager itself is constructed.
    /// This avoids unnecessary object creation for unused services.
    ///
    /// All services share the same IRepositoryManager instance,
    /// ensuring they operate within the same Unit of Work boundary.
    /// </summary>
    /// <remarks>
    /// Sealed to prevent inheritance — ServiceManager is intentionally
    /// a flat composition root, not a base class.
    /// </remarks>
    public sealed class ServiceManager : IServiceManager
    {
        // V1 BookService instance — created on first access via BookService property.
        private readonly Lazy<IBookServiceV1> _bookService;

        // V2 BookService instance — created on first access via BookServiceV2 property.
        private readonly Lazy<IBookServiceV2> _bookServiceV2;

        /// <summary>
        /// Constructs the ServiceManager and wires up all service dependencies.
        ///
        /// Dependencies are injected by the DI container and passed down
        /// to each service during lazy initialization.
        /// </summary>
        /// <param name="repositoryManager">
        /// Shared Unit of Work — gives services access to repositories and SaveAsync().
        /// </param>
        /// <param name="loggerV1">Logger scoped to V1 BookService for structured logging.</param>
        /// <param name="loggerV2">Logger scoped to V2 BookService for structured logging.</param>
        /// <param name="mapper">AutoMapper instance for DTO↔Entity mapping.</param>
        /// <param name="bookLinks">HATEOAS link generator for V1 BookDto responses.</param>
        /// <param name="bookLinksV2">HATEOAS link generator for V2 BookDtoV2 responses.</param>
        public ServiceManager(
            IRepositoryManager repositoryManager,
            ILogger<V1BookService> loggerV1,
            ILogger<V2BookService> loggerV2,
            IMapper mapper,
            IBookLinks<BookDto> bookLinks,
            IBookLinks<BookDtoV2> bookLinksV2)
        {
            if (repositoryManager is null)
                throw new ArgumentNullException(nameof(repositoryManager));

            // Wire up V1 BookService with its dependencies.
            // IBookLinks<BookDto> maps books to BookDto — no Author field.
            _bookService = new Lazy<IBookServiceV1>(
                () => new V1BookService(repositoryManager, loggerV1, mapper, bookLinks)
            );

            // Wire up V2 BookService with its dependencies.
            // IBookLinks<BookDtoV2> maps books to BookDtoV2 — includes Author field.
            _bookServiceV2 = new Lazy<IBookServiceV2>(
                () => new V2BookService(repositoryManager, loggerV2, mapper, bookLinksV2)
            );
        }

        /// <summary>
        /// Provides access to the V1 book service.
        ///
        /// V1 is deprecated — it returns books without the Author field.
        /// Kept alive to avoid breaking existing V1 clients during the migration period.
        /// New development should use BookServiceV2.
        /// </summary>
        public IBookServiceV1 BookService => _bookService.Value;

        /// <summary>
        /// Provides access to the V2 book service.
        ///
        /// V2 is the active version — it returns books including the Author field.
        /// All new features and endpoints should be built against this service.
        /// </summary>
        public IBookServiceV2 BookServiceV2 => _bookServiceV2.Value;
    }
}