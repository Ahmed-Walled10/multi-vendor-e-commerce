using System.Security.Claims;
using ECommerce.Application.Features.Authentication.Commands.Login;
using ECommerce.Application.Features.Authentication.Commands.Logout;
using ECommerce.Application.Features.Authentication.Commands.RefreshToken;
using ECommerce.Application.Features.Authentication.Commands.Register;
using ECommerce.Application.Features.Authentication.Commands.RevokeToken;
using ECommerce.Application.Features.Authentication.Commands.ConfirmEmail;
using ECommerce.Application.Features.Authentication.Commands.ResendEmailConfirmationOtp;
using ECommerce.Application.Features.Authentication.Commands.ForgotPassword;
using ECommerce.Application.Features.Authentication.Commands.ResetPassword;
using ECommerce.Application.Features.Authentication.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string? CurrentUserId =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private string? UserAgent =>
        Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : null;

    private string? ClientIp =>
        HttpContext.Connection.RemoteIpAddress?.ToString();

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        command.DeviceInfo = UserAgent;
        command.IpAddress  = ClientIp;

        var result = await _mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userId = CurrentUserId!;
        await _mediator.Send(new LogoutCommand { UserId = userId });
        return Ok(new { message = "Logged out successfully. All sessions have been revoked." });
    }


    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenCommand command)
    {
        command.RequestingUserId = CurrentUserId;
        await _mediator.Send(command);
        return Ok(new { message = "Token revoked successfully." });
    }



    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result)
        {
            return BadRequest(new { message = "Invalid email, expired OTP, or too many failed attempts." });
        }
        return Ok(new { message = "Email confirmed successfully." });
    }



    [HttpPost("resend-confirmation-otp")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationOtp([FromBody] ResendEmailConfirmationOtpCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result)
        {
            return BadRequest(new { message = "Failed to resend confirmation OTP. User may not exist or email is already confirmed." });
        }
        return Ok(new { message = "Confirmation OTP resent successfully." });
    }



    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "If the email is registered and confirmed, a password reset OTP will be sent." });
    }



    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result)
        {
            return BadRequest(new { message = "Invalid email, incorrect OTP, expired OTP, or passwords do not match requirements." });
        }
        return Ok(new { message = "Password reset successfully." });
    }
}
