using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs.Auth;
using BookDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookDemo.Infrastructure.Identity
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly RepositoryContext _context;

        public RefreshTokenRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshTokenDataDto token)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = token.TokenHash,
                UserId = token.UserId,
                CreatedAt = token.CreatedAt,
                ExpiresAt = token.ExpiresAt,
                IsRevoked = token.IsRevoked,
                RevokedAt = token.RevokedAt,
                ReplacedByTokenHash = token.ReplacedByTokenHash
            });

            await _context.SaveChangesAsync();
        }

        public async Task<RefreshTokenDataDto?> GetByHashAsync(string tokenHash)
        {
            var entity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (entity is null) return null;

            return new RefreshTokenDataDto(
                entity.TokenHash, entity.UserId, entity.CreatedAt,
                entity.ExpiresAt, entity.IsRevoked, entity.RevokedAt,
                entity.ReplacedByTokenHash);
        }

        public async Task RevokeAsync(string tokenHash, string? replacedByTokenHash = null)
        {
            var entity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (entity is null) return;

            entity.IsRevoked = true;
            entity.RevokedAt = DateTime.UtcNow;
            entity.ReplacedByTokenHash = replacedByTokenHash;

            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllForUserAsync(string userId)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
