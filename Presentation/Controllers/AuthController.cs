using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthController(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _identityService.CreateUserAsync(
                request.Email, request.Password, request.FirstName, request.LastName);
            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResponse(
                    Succeeded: false,
                    UserId: null,
                    Errors: result.Errors
                ));
            }

            // Every new user starts with the "User" role by default.
            // "Admin" role is assigned separately (see AssignRole below),
            // never automatically at registration.
            await _identityService.AddToRoleAsync(result.UserId, "User");

            return Ok(new RegisterResponse(
                Succeeded: true,
                UserId: result.UserId
            ));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var isPasswordValid = await _identityService.CheckPasswordAsync(request.Email, request.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new LoginResponse(
                    Succeeded: false,
                    Token: null,
                    Expires: null,
                    UserId: null,
                    Errors: new[] { "Invalid email or password." }
                ));
            }

            var userId = await _identityService.GetUserIdAsync(request.Email);
            var roles = await _identityService.GetRolesAsync(userId!);

            var tokenData = new UserTokenData(userId!, request.Email, roles);
            var tokenResult = _tokenService.GenerateToken(tokenData);

            return Ok(new LoginResponse(
                Succeeded: true,
                Token: tokenResult.Token,
                Expires: tokenResult.Expires,
                UserId: userId
            ));
        }

        // Only Admins can assign roles — prevents privilege escalation by
        // regular users. Still worth hardening further in a real deployment
        // (e.g. restricting which roles can be assigned, audit logging).
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromQuery] string email, [FromQuery] string role)
        {
            var userId = await _identityService.GetUserIdAsync(email);
            if (userId is null)
                return NotFound("User not found.");

            await _identityService.AddToRoleAsync(userId, role);
            return Ok($"Role '{role}' assigned to '{email}'.");
        }

    }
}

