namespace ECommerce.Application.Contracts.Infrastructure;

public interface IJwtTokenGeneration
{
    string GenerateAccessToken(string userId, string email);

    string GenerateRawRefreshToken();

    string HashToken(string rawToken);

    int RefreshTokenExpiryDays { get; }
}

