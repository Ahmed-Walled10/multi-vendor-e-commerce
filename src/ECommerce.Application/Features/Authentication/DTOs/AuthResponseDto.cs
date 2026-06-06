namespace ECommerce.Application.Features.Authentication.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
}
