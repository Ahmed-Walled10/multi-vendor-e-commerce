namespace ECommerce.Application.Contracts.Infrastructure;

public interface IOtpService
{
    string GenerateOtp();
    bool ValidateOtp(string providedOtp, string? storedOtp, DateTime? expiresAt);
}
