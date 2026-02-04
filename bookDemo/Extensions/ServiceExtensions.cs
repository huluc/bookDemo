using BookDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookDemo.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Configures the DbContext to use SQL Server with the connection string from configuration.
            services.AddDbContext<RepositoryContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("sqlConnection")));

        }
    }
}
