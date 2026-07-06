using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{
    public record RegisterResponse(
        bool Succeeded,
        string? UserId,
        IEnumerable<string>? Errors = null);
}
