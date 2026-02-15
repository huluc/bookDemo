@echo off
dotnet ef --startup-project .\BookDemo.API.csproj --project ..\BookDemo.Infrastructure\BookDemo.Infrastructure.csproj %*