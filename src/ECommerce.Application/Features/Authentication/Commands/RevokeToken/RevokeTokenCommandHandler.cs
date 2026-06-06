using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.RevokeToken;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGeneration     _jwtTokenGeneration;

    public RevokeTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGeneration     jwtTokenGeneration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGeneration     = jwtTokenGeneration;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var hash   = _jwtTokenGeneration.HashToken(request.RefreshToken);
        var stored = await _refreshTokenRepository.GetActiveByHashAsync(hash, cancellationToken)
            ?? throw new NotFoundException("RefreshToken", request.RefreshToken[..8] + "...");

        // Security: ensure the token actually belongs to the requesting user
        if (request.RequestingUserId is not null && stored.UserId != request.RequestingUserId)
            throw new ForbiddenException("You are not authorized to revoke this token.");

        stored.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
    }
}
