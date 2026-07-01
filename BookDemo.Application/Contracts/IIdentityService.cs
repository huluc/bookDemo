using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    public interface IIdentityService
    {
        Task<(bool Succeeded, string UserId)> CreateUserAsync(string email, string password);
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task<bool> IsInRoleAsync(string userId, string role);
    }
}
