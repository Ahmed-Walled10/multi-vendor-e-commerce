using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Integration.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "UseInMemoryDatabase", "True" },
                { "Jwt:Key", "1)@+nFM;b>Zr*zAK!KKQS!bKKEMJkPZ9}a20!lo:7gF" },
                { "Jwt:Issuer", "ECommerce.Api" },
                { "Jwt:Audience", "ECommerce.Users" },
                { "Jwt:AccessTokenExpiryMinutes", "15" },
                { "Jwt:RefreshTokenExpiryDays", "30" }
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove and stub IEmailService to avoid connecting to real SMTP servers
            var emailDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IEmailService));

            if (emailDescriptor != null)
            {
                services.Remove(emailDescriptor);
            }

            services.AddTransient<IEmailService, TestEmailService>();
        });
    }
}

public class TestEmailService : IEmailService
{
    public Task SendEmailConfirmationOtpAsync(string email, string firstName, string otp) => Task.CompletedTask;
    public Task SendPasswordResetOtpAsync(string email, string firstName, string otp) => Task.CompletedTask;
}
