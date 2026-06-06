using ECommerce.Application.Contracts.Persistence;
using ECommerce.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public async Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(
                t => t.TokenHash  == tokenHash &&
                     t.RevokedAt  == null       &&
                     t.ExpiresAt  > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Where(t => t.UserId    == userId &&
                        t.RevokedAt == null    &&
                        t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
