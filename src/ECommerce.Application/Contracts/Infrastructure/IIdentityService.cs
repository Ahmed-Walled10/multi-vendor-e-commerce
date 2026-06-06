namespace ECommerce.Application.Contracts.Infrastructure;

public interface IIdentityService
{
    Task<(bool Succeeded, string UserId, string[] Errors)> CreateUserAsync(string email, string password);
    Task<(bool Succeeded, string UserId)> ValidateCredentialsAsync(string email, string password);
    Task<string?> GetUserEmailAsync(string userId);
    Task<bool> UserExistsAsync(string email);
}
