using BookDemo.API.Extensions;
using BookDemo.Application.Contracts;
using BookDemo.Infrastructure.Persistence;
using BookDemo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// When RepositoryContext is requested, the DI container knows how to construct it:
// it uses the configured connection string, the SQL Server provider,
// and the defined service lifetime to create and manage the DbContext instance.
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.ConfigureRepositoryManager();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
