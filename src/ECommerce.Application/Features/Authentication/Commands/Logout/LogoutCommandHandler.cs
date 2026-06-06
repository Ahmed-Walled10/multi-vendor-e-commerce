using ECommerce.Application.Contracts.Persistence;
using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Revoke every active refresh token for this user (all devices)
        var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);

        foreach (var token in activeTokens)
            token.RevokedAt = DateTime.UtcNow;

        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }
}
