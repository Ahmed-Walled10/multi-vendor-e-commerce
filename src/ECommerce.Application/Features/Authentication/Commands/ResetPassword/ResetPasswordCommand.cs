using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<bool>
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
