using BookDemo.API.Extensions;
using BookDemo.Application.Contracts;
using BookDemo.Application.Mapping;
using BookDemo.Infrastructure.Services;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// 1) Clean  Default logging providers
builder.Logging.ClearProviders();

// 2) Connect NLog as the logging provider for the application
builder.Host.UseNLog();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true; // Respect the Accept header sent by clients
    options.ReturnHttpNotAcceptable = true; // If the client requests an unsupported media type, return 406 Not Acceptable
})
    .AddCustomCsvFormatter()
    .AddXmlDataContractSerializerFormatters()
    .AddNewtonsoftJson();

// When RepositoryContext is requested, the DI container knows how to construct it:
// it uses the configured connection string, the SQL Server provider,
// and the defined service lifetime to create and manage the DbContext instance.
builder.Services
    .ConfigureSqlContext(builder.Configuration)
    .ConfigureRepositoryManager()
    .ConfigureServiceManager()
    .ConfigureActionFilters()
    .ConfigureCors(builder.Configuration)
    .ConfigureDataShaper()
    .ConfigureBookLinks()
    .AddCustomMediaTypes()
    .AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly)
    .ConfigureVersioning();


var app = builder.Build();

app.UseGlobalExceptionHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
