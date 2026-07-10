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

        public AuthService(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
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
                    Succeeded: false,
                    Token: null,
                    Expires: null,
                    UserId: null,
                    Errors: new[] { "Invalid email or password." });
            }

            var userId = await _identityService.GetUserIdAsync(request.Email);
            var roles = await _identityService.GetRolesAsync(userId!);

            var tokenData = new UserTokenDataDto(userId!, request.Email, roles);
            var tokenResult = _tokenService.GenerateToken(tokenData);

            return new LoginResponseDto(
                Succeeded: true,
                Token: tokenResult.Token,
                Expires: tokenResult.Expires,
                UserId: userId);
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
    }
}
