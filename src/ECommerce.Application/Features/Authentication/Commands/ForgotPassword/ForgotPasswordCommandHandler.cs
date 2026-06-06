using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IOtpService _otpService;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IOtpService otpService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _otpService = otpService;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.EmailConfirmed)
        {
            // Return true to prevent user enumeration
            return true;
        }

        var otp = _otpService.GenerateOtp();
        user.PasswordResetOtp = otp;
        user.PasswordResetOtpExpiration = DateTime.UtcNow.AddMinutes(15);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to update user with password reset OTP.");
        }

        await _emailService.SendPasswordResetOtpAsync(
            user.Email!,
            user.FirstName,
            otp);

        return true;
    }
}
