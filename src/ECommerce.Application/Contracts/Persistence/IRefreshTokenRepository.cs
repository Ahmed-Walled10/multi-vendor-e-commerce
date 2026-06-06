using ECommerce.Domain.Entities.Identity;

namespace ECommerce.Application.Contracts.Persistence;

/// <summary>
/// Repository for persisted refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
