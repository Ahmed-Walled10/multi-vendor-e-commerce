using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOtpService _otpService;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IOtpService otpService)
    {
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return false;
        }

        // Validate Reset Password OTP
        if (!_otpService.ValidateOtp(request.Otp, user.PasswordResetOtp, user.PasswordResetOtpExpiration))
        {
            return false;
        }

        // Clear OTP fields
        user.PasswordResetOtp = null;
        user.PasswordResetOtpExpiration = null;

        // Reset password using standard ASP.NET Identity UserManager which verifies password strength policies
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        return result.Succeeded;
    }
}
