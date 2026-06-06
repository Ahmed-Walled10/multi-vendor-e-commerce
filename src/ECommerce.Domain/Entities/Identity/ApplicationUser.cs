using Microsoft.AspNetCore.Identity;
using ECommerce.Domain.Entities.Catalog;

namespace ECommerce.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool IsSuspended { get; set; } = false;
    public string? SuspensionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }


    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public string? EmailConfirmationOtp { get; set; }
    public DateTime? EmailConfirmationOtpExpiration { get; set; }
    public int OtpAttempts { get; set; }
    public DateTime? LastOtpAttemptAt { get; set; }

    public string? PasswordResetOtp { get; set; }
    public DateTime? PasswordResetOtpExpiration { get; set; }
}
