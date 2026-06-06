using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<bool>
{
    public string Email { get; set; } = string.Empty;
}
