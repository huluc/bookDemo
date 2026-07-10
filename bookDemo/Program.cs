using BookDemo.API.Extensions;
using BookDemo.Application.Mapping;
using BookDemo.Application.Options;
using BookDemo.Infrastructure.Identity;
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
    .ConfigureVersioning()
    .ConfigureResponseCaching()
    .ConfigureHttpCacheHeaders()
    .ConfigureHybridCache()
    .ConfigureRateLimiting()
    .ConfigureIdentity()
    .ConfigureJwtAuthentication(builder.Configuration)
    .Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


var app = builder.Build();

// Seed roles on startup (idempotent — checks existence before creating)
using (var scope = app.Services.CreateScope())
{
    await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
}

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

// ─── Middleware Pipeline Order ──────────────────────────────────────────
// Order matters here: each middleware only sees what the previous ones
// let through, and some depend on state set by the ones before them.

app.UseCors("CorsPolicy");
// CORS runs first: reject cross-origin requests early, before spending
// any work on authenticating a request we're going to block anyway.

app.UseAuthentication();
// Authentication answers "who is this?" — reads the JWT (if present),
// validates its signature/issuer/audience/expiry, and populates
// HttpContext.User with the resulting claims. Must run before
// UseAuthorization(), and before any middleware that might need to
// know the caller's identity (e.g. per-user rate limiting later).

app.UseRateLimiter();
// Currently partitioned per-IP, so its position relative to
// authentication doesn't matter yet. If this becomes per-user
// (partitioned by claims from HttpContext.User), it MUST move to
// after UseAuthentication().

app.UseResponseCaching();
// Layer 1 HTTP caching — serves full cached responses when applicable.

app.UseHttpCacheHeaders();
// Layer 2 HTTP caching — writes Cache-Control/ETag/Last-Modified headers,
// enables 304 Not Modified via conditional requests.

app.UseAuthorization();
// Authorization answers "is this identity allowed to do this?" — checks
// [Authorize]/[Authorize(Roles = "...")] against HttpContext.User.
// Must run after UseAuthentication(): without an authenticated identity,
// every authorization check would fail regardless of the actual user.

app.MapControllers();
// Endpoint execution — by this point, identity and permissions are
// already resolved; this is where the actual controller action runs.

app.MapGet("/health", () => Results.Ok("healthy"))
   .DisableRateLimiting();

app.Run();
