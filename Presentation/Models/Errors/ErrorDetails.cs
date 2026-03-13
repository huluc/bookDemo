using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Presentation.Models.Errors
{
    /// Standard error model for global exception handling.
    /// Designed as immutable and null-safe.
    /// Middleware is responsible for JSON serialization.
    /// Model sadece “data” taşısın, serialize “altyapı işi” olsun.
    public sealed class ErrorDetails
    {
        public int StatusCode { get; init; }

        // Init-only to preserve immutability and avoid null warnings.
        public string Message { get; init; } = string.Empty;
        public string? TraceId { get; init; }

        // Human-readable format for debugging and logging.
        // Not used for HTTP responses.
        //debug için bıraktim  JSON serialize işini middleware’de yapacağız.
        public override string ToString() => $"{StatusCode} - {Message}";
    }
}
