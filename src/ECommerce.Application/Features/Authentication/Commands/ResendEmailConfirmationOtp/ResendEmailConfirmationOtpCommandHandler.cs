using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Authentication.Commands.ResendEmailConfirmationOtp;

public class ResendEmailConfirmationOtpCommandHandler : IRequestHandler<ResendEmailConfirmationOtpCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public ResendEmailConfirmationOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IOtpService otpService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _otpService = otpService;
        _emailService = emailService;
    }

    public async Task<bool> Handle(ResendEmailConfirmationOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || user.EmailConfirmed)
        {
            return false;
        }

        var otp = _otpService.GenerateOtp();
        user.EmailConfirmationOtp = otp;
        user.EmailConfirmationOtpExpiration = DateTime.UtcNow.AddMinutes(10);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return false;
        }

        await _emailService.SendEmailConfirmationOtpAsync(
            user.Email!,
            user.FirstName,
            otp);

        return true;
    }
}
