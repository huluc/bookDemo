using BookDemo.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    // Orchestrates IIdentityService + ITokenService for register/login flows,
    // mirroring the Controller -> Service pattern used for Books
    // (IServiceManager -> IBookService). Keeps AuthController thin and
    // keeps multi-step orchestration (check password -> get roles ->
    // generate token) out of the Presentation layer.
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<AssignRoleResponseDto> AssignRoleAsync(string email, string role);

    }
}
