using BookDemo.Application.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task AddToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new InvalidOperationException($"User with id '{userId}' not found.");

            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password, string firstName, string lastName)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            var result = await _userManager.CreateAsync(user, password);

            return (result.Succeeded, user.Id, result.Errors.Select(e => e.Description));

        }

        public async Task<string?> GetUserIdAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.Id;
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return false;

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Enumerable.Empty<string>();

            return await _userManager.GetRolesAsync(user);
        }
    }
}
