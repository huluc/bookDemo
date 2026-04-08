using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Presentation.Filters
{
    public record LogDetails(
        string? Controller,
        string? Action,
        string? HttpMethod,
        string? Path,
        int? StatusCode,
        string? TraceId,
        string? ParameterTypes)
    {
        public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    }
}