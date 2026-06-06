using ECommerce.Application.Features.Authentication.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; set; } = default!;

    // ── Device context (populated by the controller) ──
    public string? DeviceInfo { get; set; }
    public string? IpAddress  { get; set; }
}
