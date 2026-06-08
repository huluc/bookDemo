using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookDemo.Application.DTOs
{
    public abstract class BookRequestDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [MinLength(2, ErrorMessage = "Title must be at least 2 characters long.")]
        [MaxLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        public string Title { get; set; }

        [Required]
        [Range(10,1000)]
        public decimal Price { get; set; }

        public string? Author { get; set; }
    }
}
