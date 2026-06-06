using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Revokes ALL active refresh tokens for the current user (full logout from all devices).
/// </summary>
public class LogoutCommand : IRequest
{
    public string UserId { get; set; } = default!;
}
