using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Authentication.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOtpService _otpService;

    public ConfirmEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        IOtpService otpService)
    {
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return false;
        }

        // Limit OTP attempts: if they've attempted 10 times in 15 minutes, lock them out temporarily
        if (user.OtpAttempts >= 10 &&
            user.LastOtpAttemptAt.HasValue &&
            DateTime.UtcNow < user.LastOtpAttemptAt.Value.AddMinutes(15))
        {
            return false;
        }

        if (!user.LastOtpAttemptAt.HasValue ||
            DateTime.UtcNow >= user.LastOtpAttemptAt.Value.AddMinutes(15))
        {
            user.OtpAttempts = 0;
        }

        user.OtpAttempts++;
        user.LastOtpAttemptAt = DateTime.UtcNow;

        if (!_otpService.ValidateOtp(request.Otp, user.EmailConfirmationOtp, user.EmailConfirmationOtpExpiration))
        {
            await _userManager.UpdateAsync(user);
            return false;
        }

        user.EmailConfirmed = true;
        user.EmailConfirmationOtp = null;
        user.EmailConfirmationOtpExpiration = null;
        user.OtpAttempts = 0;
        user.LastOtpAttemptAt = null;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
