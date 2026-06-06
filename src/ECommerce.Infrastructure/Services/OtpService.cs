using System.Security.Cryptography;
using ECommerce.Application.Contracts.Infrastructure;

namespace ECommerce.Infrastructure.Services;

public class OtpService : IOtpService
{
    public string GenerateOtp()
    {
        // Cryptographically secure RNG to generate 6-digit OTP
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }

    public bool ValidateOtp(string providedOtp, string? storedOtp, DateTime? expiresAt)
    {
        if (string.IsNullOrEmpty(providedOtp) || string.IsNullOrEmpty(storedOtp))
            return false;

        if (expiresAt == null || DateTime.UtcNow > expiresAt)
            return false;

        return providedOtp == storedOtp;
    }
}
