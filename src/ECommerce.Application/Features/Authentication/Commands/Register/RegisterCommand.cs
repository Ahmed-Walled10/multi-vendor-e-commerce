using ECommerce.Application.Features.Authentication.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Authentication.Commands.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string Email           { get; set; } = default!;
    public string Password        { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public string FirstName       { get; set; } = default!;
    public string LastName        { get; set; } = default!;
}
