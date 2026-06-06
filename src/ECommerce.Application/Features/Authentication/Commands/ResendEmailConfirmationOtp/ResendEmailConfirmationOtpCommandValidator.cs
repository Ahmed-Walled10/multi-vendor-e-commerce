using FluentValidation;

namespace ECommerce.Application.Features.Authentication.Commands.ResendEmailConfirmationOtp;

public class ResendEmailConfirmationOtpCommandValidator : AbstractValidator<ResendEmailConfirmationOtpCommand>
{
    public ResendEmailConfirmationOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
