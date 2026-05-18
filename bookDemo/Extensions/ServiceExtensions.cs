using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
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
        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
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
                    .OfType<SystemTextJsonOutputFormatter>()?
                    .FirstOrDefault();
                if (systemTextJsonOutputFormatter != null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.hilal.bookdemo.hateoas+json");
                }

                var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?
                .FirstOrDefault();
                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.hilal.bookdemo.hateoas+xml");
                }

                var newtonsoftOutputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()?
                .FirstOrDefault();

                if (newtonsoftOutputFormatter != null)
                {
                    newtonsoftOutputFormatter.SupportedMediaTypes
                        .Add("application/vnd.hilal.bookdemo.hateoas+json");
                }
            });

            return services;
        }
    }
}