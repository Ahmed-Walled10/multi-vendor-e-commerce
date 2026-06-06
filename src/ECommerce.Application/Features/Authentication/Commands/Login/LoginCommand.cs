using ECommerce.Application.Features.Authentication.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email    { get; set; } = default!;
    public string Password { get; set; } = default!;

}
