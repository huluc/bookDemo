using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs
{
    public record CategoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
