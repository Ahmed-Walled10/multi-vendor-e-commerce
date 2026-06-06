using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.ResendEmailConfirmationOtp;

public class ResendEmailConfirmationOtpCommand : IRequest<bool>
{
    public string Email { get; set; } = string.Empty;
}
