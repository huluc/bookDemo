using BookDemo.Application.Constants;
using BookDemo.Application.Contracts;
using BookDemo.Infrastructure.DataShaping;
using BookDemo.Infrastructure.Persistence;
using BookDemo.Infrastructure.Repositories;
using BookDemo.Infrastructure.Services;
using BookDemo.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

namespace BookDemo.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Configures the DbContext to use SQL Server with the connection string from configuration.
            services.AddDbContext<RepositoryContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("sqlConnection")));
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
    }
}