using System.ComponentModel.DataAnnotations;

namespace BookDemo.Application.DTOs.Auth
{
    public record RegisterRequest
   (
        [Required, EmailAddress]
        string Email,

        [Required, MinLength(6)]
        string Password,

        [Required]
        string FirstName,

        [Required]
        string LastName);
}

