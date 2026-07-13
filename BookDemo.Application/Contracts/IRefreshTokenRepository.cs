using BookDemo.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{

    // Separate from IIdentityService because this isn't a UserManager/Identity
    // concern — RefreshToken is our own custom entity, queried directly via
    // EF Core. Named "Repository" to match the existing Book repository
    // pattern (IRepositoryManager), even though it lives outside that
    // generic repository for simplicity (Identity-adjacent infrastructure
    // already bypasses it too — see IdentityService/UserManager).
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshTokenDataDto token);
        Task<RefreshTokenDataDto?> GetByHashAsync(string tokenHash);
        Task RevokeAsync(string tokenHash, string? replacedByTokenHash = null);

        // Used for reuse-detection response: revoke every active token for a
        // user if a stolen/already-rotated token is presented again.
        Task RevokeAllForUserAsync(string userId);
    }
}
