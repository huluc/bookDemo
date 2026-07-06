namespace BookDemo.Application.Contracts
{
    public interface IIdentityService
    {
        Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
            string email, string password, string firstName, string lastName);

        Task<bool> CheckPasswordAsync(string email, string password);

        Task<string?> GetUserIdAsync(string email);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task AddToRoleAsync(string userId, string role);
    }
}
