using BookDemo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var adminEmail = configuration["AdminUser:Email"]
                ?? throw new InvalidOperationException("AdminUser:Email configuration is missing.");
            var adminPassword = configuration["AdminUser:Password"]
                ?? throw new InvalidOperationException("AdminUser:Password configuration is missing.");

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin is not null)
                return;

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
