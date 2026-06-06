using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Application.Contracts.Persistence;
using ECommerce.Application.Features.Authentication.DTOs;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RefreshTokenEntity = ECommerce.Domain.Entities.Identity.RefreshToken;

namespace ECommerce.Application.Features.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IIdentityService            _identityService;
    private readonly IJwtTokenGeneration     _jwtTokenGeneration;
    private readonly IUnitOfWork             _unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOtpService                 _otpService;
    private readonly IEmailService               _emailService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IIdentityService            identityService,
        IJwtTokenGeneration     jwtTokenGeneration,
        IUnitOfWork             unitOfWork,
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<ApplicationUser> userManager,
        IOtpService                 otpService,
        IEmailService               emailService,
        ILogger<RegisterCommandHandler> logger)
    {
        _identityService        = identityService;
        _jwtTokenGeneration     = jwtTokenGeneration;
        _unitOfWork             = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _userManager            = userManager;
        _otpService             = otpService;
        _emailService           = emailService;
        _logger                 = logger;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🚀 Initiating user registration for email: {Email}", request.Email);

        // 1. Check if user already exists
        if (await _identityService.UserExistsAsync(request.Email))
        {
            _logger.LogWarning("⚠️ User registration failed. Email already exists: {Email}", request.Email);
            throw new ConflictException($"User with email '{request.Email}' already exists.");
        }

        // 2. Create Identity user
        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(request.Email, request.Password);

        if (!succeeded)
        {
            _logger.LogWarning("⚠️ Identity user creation failed for email: {Email}. Errors: {Errors}", request.Email, string.Join(", ", errors));
            throw new ValidationException(
                errors.Select(e => new FluentValidation.Results.ValidationFailure("Password", e)));
        }

        // 3. Commit the new user
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Retrieve created user and set Email Confirmation OTP
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var otp = _otpService.GenerateOtp();
            user.EmailConfirmationOtp = otp;
            user.EmailConfirmationOtpExpiration = DateTime.UtcNow.AddMinutes(10);
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _userManager.UpdateAsync(user);

            // Send confirmation email asynchronously (fire and forget or await)
            try
            {
                await _emailService.SendEmailConfirmationOtpAsync(user.Email!, user.FirstName, otp);
                _logger.LogInformation("✉️ Sent confirmation OTP email to: {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "❌ Failed to send confirmation OTP email to {Email}, registration proceeding.", user.Email);
            }
        }

        _logger.LogInformation("✅ User registered successfully. ID: {UserId}, Email: {Email}", userId, request.Email);

        // 5. Generate access token
        var accessToken = _jwtTokenGeneration.GenerateAccessToken(userId, request.Email);

        // 6. Generate and persist refresh token
        var rawRefreshToken = _jwtTokenGeneration.GenerateRawRefreshToken();
        var tokenHash       = _jwtTokenGeneration.HashToken(rawRefreshToken);

        var refreshTokenEntity = new RefreshTokenEntity
        {
            UserId     = userId,
            TokenHash  = tokenHash,
            ExpiresAt  = DateTime.UtcNow.AddDays(_jwtTokenGeneration.RefreshTokenExpiryDays)
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken  = accessToken,
            RefreshToken = rawRefreshToken,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(15),
            UserId       = userId,
            Email        = request.Email
        };
    }
}
