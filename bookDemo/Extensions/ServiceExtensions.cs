using Asp.Versioning;
using BookDemo.Application.Constants;
using BookDemo.Application.Contracts;
using BookDemo.Infrastructure.Caching;
using BookDemo.Infrastructure.DataShaping;
using BookDemo.Infrastructure.Identity;
using BookDemo.Infrastructure.Persistence;
using BookDemo.Infrastructure.Repositories;
using BookDemo.Infrastructure.Services;
using BookDemo.Presentation.Filters;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace BookDemo.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Configures the DbContext to use SQL Server with the connection string from configuration.
            services.AddDbContext<RepositoryContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("sqlConnection"))
                       .EnableSensitiveDataLogging());
            return services; //enables method chaining by returning the IServiceCollection instance.

        }
        public static IServiceCollection ConfigureRepositoryManager(this IServiceCollection services)
        {
            // Registers the RepositoryManager as a scoped service for dependency injection.
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            return services; //enables method chaining by returning the IServiceCollection instance.

        }

        public static IServiceCollection ConfigureServiceManager(this IServiceCollection services)
        {
            // Registers the ServiceManager as a scoped service for dependency injection.
            services.AddScoped<IServiceManager, ServiceManager>();
            return services; //enables method chaining by returning the IServiceCollection instance.
        }

        public static IServiceCollection ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<LogActionAttribute>();
            services.AddScoped<ValidateMediaTypeAttribute>();
            return services;
        }
        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            // Reads the "AllowedOrigins" array from appsettings.json or appsettings.Development.json.
            // In Development: localhost ports. In Production: real site URL
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? throw new InvalidOperationException("AllowedOrigins configuration is missing.");
            services.AddCors(options =>
            {
                // Defines a CORS policy named "CorsPolicy".
                // Activated in Program.cs via app.UseCors("CorsPolicy").
                options.AddPolicy("CorsPolicy", builder =>
                    builder
                        // Only allow requests from origins defined in AllowedOrigins.
                        .WithOrigins(allowedOrigins!)
                        // Allow all HTTP methods: GET, POST, PUT, DELETE, OPTIONS etc.
                        .AllowAnyMethod()
                        // Allow all headers: Content-Type, Authorization etc.
                        .AllowAnyHeader()
                        // Expose this header so frontend can read pagination metadata.
                        .WithExposedHeaders("X-Pagination")
                );
            });
            return services;
        }

        public static IServiceCollection ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped(typeof(IDataShaper<>), typeof(DataShaper<>));
            return services;
        }

        public static IServiceCollection ConfigureBookLinks(this IServiceCollection services)
        {
            // Open generic registration — works for BookLinks<BookDto>, BookLinks<BookDtoV2>, etc.
            // No changes needed when new versions are introduced.
            services.AddScoped(typeof(IBookLinks<>), typeof(BookLinks<>));
            return services;
        }

        /// <summary>
        /// Registers custom media types for the output formatters.
        /// This allows the API to accept and process requests with custom Accept headers
        /// such as 'application/vnd.hilal.bookdemo.hateoas+json' for HATEOAS support.
        /// Without this registration, the framework would return 406 Not Acceptable
        /// for any unrecognized media type.
        /// </summary>
        public static IServiceCollection AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config.OutputFormatters
                    .OfType<SystemTextJsonOutputFormatter>()
                    .FirstOrDefault();
                if (systemTextJsonOutputFormatter != null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes.Add(MediaTypes.HateoasJson);
                }

                var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()
                .FirstOrDefault();
                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add(MediaTypes.HateoasXml);
                    xmlOutputFormatter.SupportedMediaTypes.Add(MediaTypes.ApiRootXml);
                }

                var newtonsoftOutputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()
                .FirstOrDefault();

                if (newtonsoftOutputFormatter != null)
                {
                    newtonsoftOutputFormatter.SupportedMediaTypes
                        .Add(MediaTypes.HateoasJson);
                    newtonsoftOutputFormatter.SupportedMediaTypes
                        .Add(MediaTypes.ApiRootJson);
                }
            });

            return services;
        }
        /// <summary>
        /// Configures API versioning for the application.
        /// Default version is 1.0 and version is read from URL segment,
        /// request header, or query string.
        /// </summary>
        public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("api-version"),
                    new QueryStringApiVersionReader("api-version")
                );
            })
            .AddMvc(); // ← controller'ların versiyonlama ile düzgün çalışması için gerekli

            return services;
        }

        /// <summary>
        /// Registers the server-side response caching service.
        /// Works at the HTTP layer — caches full responses in server memory.
        /// Activated in the pipeline via app.UseResponseCaching().
        /// </summary>
        public static IServiceCollection ConfigureResponseCaching(this IServiceCollection services)
        {
            services.AddResponseCaching();
            return services;
        }

        /// <summary>
        /// Configures HTTP cache headers using Marvin.Cache.Headers.
        /// Works at the HTTP layer — writes Cache-Control, ETag, and Last-Modified headers.
        /// Enables the validation model: expired cache entries are revalidated via
        /// If-None-Match / If-Modified-Since before re-downloading the full response.
        /// Does NOT prevent DB queries — use HybridCache for application-level caching.
        /// </summary>
        public static IServiceCollection ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddHttpCacheHeaders(
                expirationOptions =>
                {
                    // Browsers and proxies cache responses for 70 seconds.
                    expirationOptions.MaxAge = 70;
                    // Public: browsers, proxies, and CDNs may all cache the response.
                    expirationOptions.CacheLocation = CacheLocation.Public;
                },
                validationOptions =>
                {
                    // After max-age expires, clients must revalidate before using stale data.
                    validationOptions.MustRevalidate = true;
                });
            return services;
        }
        public static IServiceCollection ConfigureHybridCache(this IServiceCollection services)
        {
            services.AddHybridCache();
            // Registers HybridBookCache as the IBookCache implementation.
            // Used by both V1 and V2 — V1 only invalidates, V2 also reads from cache.
            services.AddScoped<IBookCache, HybridBookCache>();
            return services;
        }
        /// <summary>
        /// Configures rate limiting using the built-in .NET rate limiter (Token Bucket algorithm).
        /// Partitioned per client IP — each client gets its own bucket, so one noisy client
        /// cannot exhaust the limit for everyone else.
        /// Token bucket allows controlled bursts (up to TokenLimit) while enforcing a steady
        /// average rate via periodic replenishment.
        /// Activated in the pipeline via app.UseRateLimiter().
        /// </summary>
        public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Default rejection status when no tokens are available.
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // One token bucket per client IP address.
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            // Bucket capacity → max number of requests in a single burst.
                            TokenLimit = 100,
                            // 10 tokens added each period → sustained rate of 10 req/sec.
                            TokensPerPeriod = 10,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                            // No queueing — reject immediately when the bucket is empty.
                            // For an HTTP API a fast 429 is better than holding the request.
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                });

                // Custom rejection response: sets Retry-After so clients know when to retry.
                options.OnRejected = async (context, token) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString();
                    }

                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.", token);
                };
            });

            return services;
        }

        // AddIdentity<ApplicationUser, IdentityRole> registers a lot behind this single line:
        // UserManager<ApplicationUser>, RoleManager<IdentityRole>, SignInManager<ApplicationUser>,
        // password hasher, validators, etc. No need to register these one by one in the
        // DI container, this method handles all of it.
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password/Lockout settings below are configurable to taste, these are
                // common/reasonable defaults. RequireNonAlphanumeric = false is set here
                // because in interview/demo projects it's often practical to relax
                // password rules a bit for easier testing — tighten this if needed.

                // Password policy
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // Lockout policy
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User policy
                options.User.RequireUniqueEmail = true;
            })
            // Tells Identity "where will you store user/role data". Without this,
            // UserManager can't function because it wouldn't know which DbContext
            // to persist data through.
            .AddEntityFrameworkStores<RepositoryContext>()
            // Provides token generation for operations like email confirmation and
            // password reset. Not used right away since we're focused on login/register
            // for now, but this infrastructure will be ready if a "forgot password"
            // feature is added later.
            .AddDefaultTokenProviders();

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}