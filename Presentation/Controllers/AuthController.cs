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
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Succeeded ? Ok(result) : Unauthorized(result);
        }

        // Only Admins can assign roles — prevents privilege escalation by
        // regular users. Still worth hardening further in a real deployment
        // (e.g. restricting which roles can be assigned, audit logging).
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromQuery] string email, [FromQuery] string role)
        {
            var result = await _authService.AssignRoleAsync(email, role);
            return result.Succeeded ? Ok(result) : NotFound(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return result.Succeeded ? Ok(result) : Unauthorized(result);
        }

        // Requires authentication: an anonymous caller shouldn't be able to probe
        // arbitrary refresh token values without at least holding a valid access token.
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            var succeeded = await _authService.LogoutAsync(request);
            return succeeded ? Ok(new { message = "Logged out successfully." }) : NotFound();
        }
    }
}

