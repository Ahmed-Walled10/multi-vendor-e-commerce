using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Persistence.Repositories;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, string UserId, string[] Errors)> CreateUserAsync(string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
            return (true, user.Id, Array.Empty<string>());

        return (false, string.Empty, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string UserId)> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return (false, string.Empty);

        var isValid = await _userManager.CheckPasswordAsync(user, password);

        return isValid ? (true, user.Id) : (false, string.Empty);
    }

    public async Task<string?> GetUserEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Email;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }
}
