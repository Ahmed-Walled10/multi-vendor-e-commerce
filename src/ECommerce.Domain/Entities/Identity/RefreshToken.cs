using ECommerce.Domain.Common.Primitives;

namespace ECommerce.Domain.Entities.Identity;

public class RefreshToken : BaseEntity
{

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }

    public bool IsExpired  => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked  => RevokedAt.HasValue;
    public bool IsActive   => !IsExpired && !IsRevoked;



    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}
