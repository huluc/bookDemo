using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{

    // Keeps Application decoupled from the Infrastructure-only RefreshToken
    // entity — same reasoning as UserTokenDataDto for ApplicationUser.
    public record RefreshTokenDataDto(
        string TokenHash,
        string UserId,
        DateTime CreatedAt,
        DateTime ExpiresAt,
        bool IsRevoked,
        DateTime? RevokedAt,
        string? ReplacedByTokenHash)
    {
        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    }
}
