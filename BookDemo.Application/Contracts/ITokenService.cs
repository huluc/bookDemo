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
        TokenResult GenerateToken(UserTokenData data);
    }
}
