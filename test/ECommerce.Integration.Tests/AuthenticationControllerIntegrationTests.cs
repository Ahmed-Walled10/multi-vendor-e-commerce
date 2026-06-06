using System.Net;
using System.Net.Http.Json;
using ECommerce.Application.Features.Authentication.Commands.Register;
using ECommerce.Application.Features.Authentication.DTOs;
using Xunit;

namespace ECommerce.Integration.Tests;

public class AuthenticationControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthenticationControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidPayload_ShouldRegisterUserAndReturnTokens()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = $"integration_{Guid.NewGuid()}@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Integration",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/register", command);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(authResponse);
        Assert.Equal(command.Email, authResponse.Email);
        Assert.False(string.IsNullOrEmpty(authResponse.AccessToken));
        Assert.False(string.IsNullOrEmpty(authResponse.RefreshToken));
        Assert.False(string.IsNullOrEmpty(authResponse.UserId));
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "mismatched@test.com",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/register", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
