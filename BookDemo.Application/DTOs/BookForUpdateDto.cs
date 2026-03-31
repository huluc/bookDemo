using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookDemo.Application.DTOs
{
    public class BookForUpdateDto : BookRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}
