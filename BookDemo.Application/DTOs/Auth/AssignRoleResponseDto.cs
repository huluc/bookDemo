using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{
    public record AssignRoleResponseDto(
        bool Succeeded,
        string? Message,
        IEnumerable<string>? Errors = null);
}
