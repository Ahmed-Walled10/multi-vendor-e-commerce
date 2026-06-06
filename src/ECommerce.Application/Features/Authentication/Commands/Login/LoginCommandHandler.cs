using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Authentication.DTOs;
using ECommerce.Domain.Exceptions;
using MediatR;
using RefreshTokenEntity = ECommerce.Domain.Entities.Identity.RefreshToken;

namespace ECommerce.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService        _identityService;
    private readonly IJwtTokenGeneration     _jwtTokenGeneration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(
        IIdentityService        identityService,
        IJwtTokenGeneration     jwtTokenGeneration,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _identityService        = identityService;
        _jwtTokenGeneration     = jwtTokenGeneration;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate credentials
        var (succeeded, userId) = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);

        if (!succeeded)
            throw new ValidationException(
                [new FluentValidation.Results.ValidationFailure("Credentials", "Invalid email or password.")]);

        // 2. Generate access token (short-lived JWT)
        var accessToken = _jwtTokenGeneration.GenerateAccessToken(userId, request.Email);

        // 3. Generate opaque refresh token and persist its hash
        var rawRefreshToken = _jwtTokenGeneration.GenerateRawRefreshToken();
        var tokenHash       = _jwtTokenGeneration.HashToken(rawRefreshToken);

        var refreshTokenEntity = new RefreshTokenEntity
        {
            UserId     = userId,
            TokenHash  = tokenHash,
            ExpiresAt  = DateTime.UtcNow.AddDays(_jwtTokenGeneration.RefreshTokenExpiryDays)
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken  = accessToken,
            RefreshToken = rawRefreshToken,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(15),
            UserId       = userId,
            Email        = request.Email
        };
    }
}
