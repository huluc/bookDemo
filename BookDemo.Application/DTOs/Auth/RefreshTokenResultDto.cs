using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{

    // Mirrors TokenResultDto's pattern: the token generator returns its own
    // expiry alongside the token, so callers never hardcode/duplicate the
    // expiry calculation (same bug class we fixed earlier for access tokens).
    public record RefreshTokenResultDto(string Token, DateTime Expires);
}
