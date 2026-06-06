using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Authentication.DTOs;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IJwtTokenGeneration     _jwtTokenGeneration;
    private readonly IIdentityService        _identityService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenCommandHandler(
        IJwtTokenGeneration     jwtTokenGeneration,
        IIdentityService        identityService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtTokenGeneration     = jwtTokenGeneration;
        _identityService        = identityService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Hash the incoming token and look it up
        var incomingHash = _jwtTokenGeneration.HashToken(request.RefreshToken);
        var storedToken  = await _refreshTokenRepository.GetActiveByHashAsync(incomingHash, cancellationToken)
            ?? throw new ForbiddenException("Invalid, expired, or already-used refresh token.");

        // 2. Verify the user still exists
        var email = await _identityService.GetUserEmailAsync(storedToken.UserId)
            ?? throw new NotFoundException("User", storedToken.UserId);

        // 3. Generate new token pair
        var accessToken   = _jwtTokenGeneration.GenerateAccessToken(storedToken.UserId, email);
        var rawNewRefresh = _jwtTokenGeneration.GenerateRawRefreshToken();
        var newTokenHash  = _jwtTokenGeneration.HashToken(rawNewRefresh);

        // 4. Rotate: revoke the old token and link it to the new one
        storedToken.RevokedAt           = DateTime.UtcNow;
        storedToken.ReplacedByTokenHash = newTokenHash;

        // 5. Persist the replacement token
        var newRefreshTokenEntity = new Domain.Entities.Identity.RefreshToken
        {
            UserId     = storedToken.UserId,
            TokenHash  = newTokenHash,
            DeviceInfo = request.DeviceInfo ?? storedToken.DeviceInfo,
            IpAddress  = request.IpAddress  ?? storedToken.IpAddress,
            ExpiresAt  = DateTime.UtcNow.AddDays(_jwtTokenGeneration.RefreshTokenExpiryDays)
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken  = accessToken,
            RefreshToken = rawNewRefresh,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(15),
            UserId       = storedToken.UserId,
            Email        = email
        };
    }
}
