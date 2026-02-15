using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace BookDemo.Infrastructure.Persistence.ContextFactory
{
    /// <summary>
    /// ENGLISH:
    /// This factory is used by Entity Framework Core at design-time 
    /// (for example when running 'dotnet ef migrations add') to create 
    /// an instance of RepositoryContext.
    ///
    /// In a multi-layered / Clean Architecture setup, the DbContext is 
    /// located in the Infrastructure project while the application's 
    /// startup logic (Program.cs) lives in another project (e.g., API).
    ///
    /// During design-time, EF Core Tools cannot always resolve the DbContext
    /// through Dependency Injection (DI). Therefore, we explicitly provide
    /// a way to construct the DbContext manually using the connection string.
    ///
    /// This ensures migrations can be created reliably without depending
    /// on the application's runtime configuration.
    ///
    /// IMPORTANT:
    /// This class is ONLY used at design-time. It is NOT used when the 
    /// application runs normally.

    /// </summary>
    public sealed class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString =
                          configuration.GetConnectionString("sqlConnection");

            var optionsBuilder =
                new DbContextOptionsBuilder<RepositoryContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new RepositoryContext(optionsBuilder.Options);
        }
    }
}
