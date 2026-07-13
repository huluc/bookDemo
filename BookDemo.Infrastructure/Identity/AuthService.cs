using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IIdentityService identityService, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var result = await _identityService.CreateUserAsync(
                request.Email, request.Password, request.FirstName, request.LastName);

            if (!result.Succeeded)
            {
                return new RegisterResponseDto(
                    Succeeded: false,
                    UserId: null,
                    Errors: result.Errors);
            }

            // Every new user starts with the "User" role by default.
            // "Admin" role is assigned separately via the AssignRole endpoint,
            // never automatically at registration (privilege escalation risk).
            await _identityService.AddToRoleAsync(result.UserId, "User");

            return new RegisterResponseDto(
                Succeeded: true,
                UserId: result.UserId);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var isPasswordValid = await _identityService.CheckPasswordAsync(
                request.Email, request.Password);

            if (!isPasswordValid)
            {
                return new LoginResponseDto(
                    Succeeded: false, AccessToken: null, RefreshToken: null,
                    Expires: null,
                    UserId: null,
                    Errors: new[] { "Invalid email or password." });
            }

            var userId = await _identityService.GetUserIdAsync(request.Email);
            var roles = await _identityService.GetRolesAsync(userId!);

            var tokenData = new UserTokenDataDto(userId!, request.Email, roles);
            var accessTokenResult = _tokenService.GenerateAccessToken(tokenData);

            var refreshTokenResult = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.ComputeRefreshTokenHash(refreshTokenResult.Token);

            await _refreshTokenRepository.AddAsync(new RefreshTokenDataDto(
                TokenHash: refreshTokenHash,
                UserId: userId!,
                CreatedAt: DateTime.UtcNow,
                ExpiresAt: refreshTokenResult.Expires,
                IsRevoked: false,
                RevokedAt: null,
                ReplacedByTokenHash: null));

            return new LoginResponseDto(
                Succeeded: true,
                AccessToken: accessTokenResult.Token,
                RefreshToken: refreshTokenResult.Token,   // raw value goes to client; only the hash is stored
                Expires: accessTokenResult.Expires,
                UserId: userId
                );
        }

        public async Task<AssignRoleResponseDto> AssignRoleAsync(string email, string role)
        {
            var userId = await _identityService.GetUserIdAsync(email);
            if (userId is null)
            {
                return new AssignRoleResponseDto(
                    Succeeded: false,
                    Message: null,
                    Errors: new[] { "User not found." });
            }

            await _identityService.AddToRoleAsync(userId, role);

            return new AssignRoleResponseDto(
                Succeeded: true,
                Message: $"Role '{role}' assigned to '{email}'.");
        }
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var incomingHash = _tokenService.ComputeRefreshTokenHash(request.RefreshToken);
            var stored = await _refreshTokenRepository.GetByHashAsync(incomingHash);

            if (stored is null)
            {
                return new LoginResponseDto(false, null, null, null, null,
                    new[] { "Invalid refresh token." });
            }

            // REUSE DETECTION: a revoked token being presented again means it
            // was already rotated once (or logged out) — a legitimate client
            // only ever holds the LATEST token in the chain. This is a strong
            // signal of theft, so we respond by revoking every session the
            // user has, forcing a fresh login everywhere.
            if (stored.IsRevoked)
            {
                await _refreshTokenRepository.RevokeAllForUserAsync(stored.UserId);
                return new LoginResponseDto(false, null, null, null, null,
                    new[] { "Refresh token reuse detected. All sessions have been revoked." });
            }

            if (!stored.IsActive)
            {
                return new LoginResponseDto(false, null, null, null, null,
                    new[] { "Refresh token expired." });
            }

            var email = await _identityService.GetUserEmailAsync(stored.UserId);
            var roles = await _identityService.GetRolesAsync(stored.UserId);

            var accessTokenResult = _tokenService.GenerateAccessToken(
                new UserTokenDataDto(stored.UserId, email!, roles));

            // ROTATION: old token is revoked and linked to the new one via
            // ReplacedByTokenHash — this is what makes reuse-detection above
            // possible (we can trace "this token was already replaced").
            var newRefreshTokenResult = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = _tokenService.ComputeRefreshTokenHash(newRefreshTokenResult.Token);

            await _refreshTokenRepository.RevokeAsync(incomingHash, replacedByTokenHash: newRefreshTokenHash);

            await _refreshTokenRepository.AddAsync(new RefreshTokenDataDto(
                TokenHash: newRefreshTokenHash,
                UserId: stored.UserId,
                CreatedAt: DateTime.UtcNow,
                ExpiresAt: newRefreshTokenResult.Expires,
                IsRevoked: false,
                RevokedAt: null,
                ReplacedByTokenHash: null));

            return new LoginResponseDto(
                Succeeded: true,
                AccessToken: accessTokenResult.Token,
                RefreshToken: newRefreshTokenResult.Token,
                Expires: accessTokenResult.Expires,
                UserId: stored.UserId);
        }

        public async Task<bool> LogoutAsync(LogoutRequestDto request)
        {
            var hash = _tokenService.ComputeRefreshTokenHash(request.RefreshToken);
            var stored = await _refreshTokenRepository.GetByHashAsync(hash);

            if (stored is null) return false;

            await _refreshTokenRepository.RevokeAsync(hash);
            return true;
        }

    }
}
