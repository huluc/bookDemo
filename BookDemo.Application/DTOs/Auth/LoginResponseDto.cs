using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{
     public record LoginResponseDto(
         bool Succeeded,
         string? AccessToken,
         string? RefreshToken,
         DateTime? Expires,
         string? UserId,
         IEnumerable<string>? Errors = null);
}
