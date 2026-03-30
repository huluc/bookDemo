using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.Text;
using BookDemo.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace BookDemo.Presentation.Formatters
{
    /// <summary>
    /// Custom CSV output formatter.
    /// Converts BookDto or IEnumerable<BookDto> into CSV format.
    /// </summary>
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            // Register supported media type (Content-Type)
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));

            // Register supported encodings
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        /// <summary>
        /// Determines whether this formatter can handle the given type.
        /// Only supports BookDto and IEnumerable<BookDto>.
        /// </summary>
        protected override bool CanWriteType(Type? type)
        {
            if (type == null)
                return false;

            // Check if the type is either BookDto or a collection of BookDto
            var isBookType =
                typeof(BookDto).IsAssignableFrom(type) ||
                typeof(IEnumerable<BookDto>).IsAssignableFrom(type);

            if(!isBookType)
                return false;

            // Let base class perform additional checks (e.g., encoding support)
            return base.CanWriteType(type);

        }
        private static void WriteHeader(StringBuilder buffer)
        {
            buffer.AppendLine("Id,Title,Price");
        }

        /// <summary>
        /// Converts a single BookDto into a CSV row.
        /// </summary>
        private static void FormatCsv(StringBuilder buffer, BookDto book)
        {
            // Ensure the Title field is safe for CSV format
            var title = EscapeCsvField(book.Title);

            buffer.AppendLine($"{book.Id},{title},{book.Price}");
        }

        /// <summary>
        /// Escapes CSV special characters (comma, quotes, newline).
        /// Wraps field in quotes if needed and escapes inner quotes.
        /// </summary>
        private static string EscapeCsvField(string? field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // If field contains special characters, escape it according to CSV rules
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }

        /// <summary>
        /// Writes the response body in CSV format.
        /// Handles both single object and collection scenarios.
        /// </summary>
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            WriteHeader(buffer);

            if (context.Object is IEnumerable<BookDto> books)
            {
                foreach (var book in books)
                {
                    FormatCsv(buffer, book);
                }
            }
            else if (context.Object is BookDto book)
            {
                FormatCsv(buffer, book);
            }
            await response.WriteAsync(buffer.ToString(), selectedEncoding);
        }
    }
}
