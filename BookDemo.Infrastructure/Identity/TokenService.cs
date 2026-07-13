using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs.Auth;
using BookDemo.Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookDemo.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        // IOptions<JwtSettings> is resolved from DI because Program.cs calls
        // Configure<JwtSettings>(...), which binds the "JwtSettings" section
        // of appsettings.json and registers it as IOptions<JwtSettings>.
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public TokenResultDto GenerateAccessToken(UserTokenDataDto data)
        {
            // Claims are the pieces of data embedded inside the token.
            // Sub (subject) = who the token belongs to (user id).
            // Jti (JWT ID) = unique id for this token, useful later for
            // revocation/blacklisting if needed.
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, data.UserId),
            new(JwtRegisteredClaimNames.Email, data.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // ClaimTypes.Role is important: [Authorize(Roles = "Admin")] checks
            // claims of this exact type. Without this, role-based authorization
            // would not work even if the user has roles in the database.
            claims.AddRange(data.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Symmetric key: the same secret is used both to sign and to validate
            // the token (as opposed to asymmetric/RSA, which uses separate
            // public/private keys — not needed at this scale).
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            // Signing algorithm. The signature guarantees the token wasn't
            // tampered with — if a client edits the payload (e.g. changes the
            // role to "Admin"), the signature won't match anymore and the
            // server will reject the token.
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            // Serializes the token object into the final JWT string
            // (header.payload.signature, base64-encoded).
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResultDto(tokenString, expires);
        }
        public RefreshTokenResultDto GenerateRefreshToken()
        {
            // 64 random bytes, Base64-encoded. High entropy is enough here —
            // unlike passwords, this isn't user-chosen/low-entropy, so we don't
            // need a slow/salted algorithm.
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(randomBytes);
            var expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            return new RefreshTokenResultDto(token, expires);
        }

        public string ComputeRefreshTokenHash(string token)
        {
            // Same principle as password hashing: never persist the raw token.
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
