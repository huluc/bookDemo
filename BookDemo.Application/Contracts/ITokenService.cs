using BookDemo.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    // Kept separate from IIdentityService (Single Responsibility):
    // IIdentityService manages users (create, check password, roles),
    // while token generation is a distinct, JWT-specific concern.
    // Swapping JWT for another mechanism (e.g. opaque token + Redis)
    // only requires changing this service, not IIdentityService.
    public interface ITokenService
    {
        // Renamed from GenerateToken -> GenerateAccessToken now that this
        // service also issues refresh tokens; the specific name avoids ambiguity.
        TokenResultDto GenerateAccessToken(UserTokenDataDto data);

        // Refresh tokens are NOT JWTs — no claims, no structure, just a long
        // random opaque string. Keeping them opaque (vs. a JWT) means we can
        // revoke them INSTANTLY by flipping a database flag. A JWT refresh
        // token would still "look valid" until its own expiry even after
        // revocation, unless we maintained a blocklist anyway — which would
        // defeat the point of using a stateless token in the first place.
        RefreshTokenResultDto GenerateRefreshToken();

        // Centralizes the hashing logic so it's computed identically whether
        // we're storing a new token or validating an incoming one.
        string ComputeRefreshTokenHash(string token);

    }
}
