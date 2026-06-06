using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.RevokeToken;

/// <summary>
/// Revokes a specific refresh token (single-device sign-out or admin revocation).
/// </summary>
public class RevokeTokenCommand : IRequest
{
    public string RefreshToken { get; set; } = default!;

    /// <summary>
    /// If provided, the token must belong to this user (prevents cross-user revocation).
    /// </summary>
    public string? RequestingUserId { get; set; }
}
