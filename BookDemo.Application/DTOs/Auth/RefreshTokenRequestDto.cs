using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{
    public record RefreshTokenRequestDto(
        [Required] string RefreshToken);
}
