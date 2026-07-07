using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs.Auth;
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

    }
}

