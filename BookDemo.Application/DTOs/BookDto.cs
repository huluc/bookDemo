using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs
{
    public record BookDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public decimal Price { get; init; }
    }
}
