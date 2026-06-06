using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ECommerce.Application.Contracts.Infrastructure;
using ECommerce.Infrastructure.Options;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ECommerce.Infrastructure.Mail;

public class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendEmailConfirmationOtpAsync(string email, string firstName, string otp)
    {
        var subject = "Confirm Your Email - Cloud Co E-Commerce";
        var body = $@"
            <!DOCTYPE html>
         <html lang=""en"">
         <head>
           <meta charset=""UTF-8"" />
           <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
           <title>Verify Your Email</title>
           <style>
             body {{ margin: 0; padding: 0; background: #f4f4f4; font-family: Arial, sans-serif; }}
             .wrap {{ max-width: 520px; margin: 40px auto; background: #ffffff; border-radius: 6px; overflow: hidden; border: 1px solid #ddd; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
             .top-bar {{ background: #0078d4; height: 4px; }}
             .body {{ padding: 40px 40px 32px; }}
             .logo {{ font-size: 22px; font-weight: 900; color: #0078d4; letter-spacing: 1px; margin-bottom: 28px; }}
             h1 {{ font-size: 22px; color: #333333; margin: 0 0 12px; }}
             p {{ font-size: 15px; color: #666666; line-height: 1.6; margin: 0 0 28px; }}
             p strong {{ color: #333333; font-weight: 600; }}
             .otp-box {{ background: #f9f9f9; border: 1px solid #eaeaea; border-radius: 6px; text-align: center; padding: 24px 20px; margin-bottom: 24px; }}
             .otp-box .otp {{ font-size: 38px; font-weight: 700; letter-spacing: 14px; color: #0078d4; }}
             .otp-box .expiry {{ font-size: 12px; color: #888; margin-top: 10px; }}
             .note {{ font-size: 13px; color: #888; border-top: 1px solid #eee; padding-top: 20px; margin: 0; }}
             .footer {{ background: #f4f4f4; padding: 16px 40px; font-size: 11px; color: #666; border-top: 1px solid #eee; }}
           </style>
         </head>
         <body>
           <div class=""wrap"">
             <div class=""top-bar""></div>
             <div class=""body"">
               <div class=""logo"">CLOUD CO E-COMMERCE</div>
               <h1>Verify your email</h1>
               <p>Hi <strong>{firstName}</strong>, enter the code below to verify your email address and activate your account.</p>
               <div class=""otp-box"">
                 <div class=""otp"">{otp}</div>
                 <div class=""expiry"">Expires in 15 minutes · Do not share this code</div>
               </div>
               <p class=""note"">If you didn't create a Cloud Co account, you can safely ignore this email.</p>
             </div>
             <div class=""footer"">© 2026 Cloud Co E-Commerce &nbsp;·&nbsp; Privacy Policy &nbsp;·&nbsp; Help Center</div>
           </div>
         </body>
         </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetOtpAsync(string email, string firstName, string otp)
    {
        var subject = "Reset Your Password - Cloud Co E-Commerce";
        var body = $@"
            <!DOCTYPE html>
         <html lang=""en"">
         <head>
           <meta charset=""UTF-8"" />
           <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
           <title>Reset Your Password</title>
           <style>
             body {{ margin: 0; padding: 0; background: #f4f4f4; font-family: Arial, sans-serif; }}
             .wrap {{ max-width: 520px; margin: 40px auto; background: #ffffff; border-radius: 6px; overflow: hidden; border: 1px solid #ddd; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
             .top-bar {{ background: #0078d4; height: 4px; }}
             .body {{ padding: 40px 40px 32px; }}
             .logo {{ font-size: 22px; font-weight: 900; color: #0078d4; letter-spacing: 1px; margin-bottom: 28px; }}
             h1 {{ font-size: 22px; color: #333333; margin: 0 0 12px; }}
             p {{ font-size: 15px; color: #666666; line-height: 1.6; margin: 0 0 28px; }}
             p strong {{ color: #333333; font-weight: 600; }}
             .otp-box {{ background: #f9f9f9; border: 1px solid #eaeaea; border-radius: 6px; text-align: center; padding: 24px 20px; margin-bottom: 24px; }}
             .otp-box .otp {{ font-size: 38px; font-weight: 700; letter-spacing: 14px; color: #0078d4; }}
             .otp-box .expiry {{ font-size: 12px; color: #888; margin-top: 10px; }}
             .note {{ font-size: 13px; color: #888; border-top: 1px solid #eee; padding-top: 20px; margin: 0; }}
             .footer {{ background: #f4f4f4; padding: 16px 40px; font-size: 11px; color: #666; border-top: 1px solid #eee; }}
           </style>
         </head>
         <body>
           <div class=""wrap"">
             <div class=""top-bar""></div>
             <div class=""body"">
               <div class=""logo"">CLOUD CO E-COMMERCE</div>
               <h1>Reset your password</h1>
               <p>Hi <strong>{firstName}</strong>, we received a request to reset your password. Use the code below to continue. If this wasn't you, ignore this email — your password won't change.</p>
               <div class=""otp-box"">
                 <div class=""otp"">{otp}</div>
                 <div class=""expiry"">Expires in 15 minutes · Do not share this code</div>
               </div>
               <p class=""note"">For security, never share this code with anyone. Cloud Co staff will never ask for it.</p>
             </div>
             <div class=""footer"">© 2026 Cloud Co E-Commerce &nbsp;·&nbsp; Privacy Policy &nbsp;·&nbsp; Help Center</div>
           </div>
         </body>
         </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // Use regular expression to find if there is an OTP inside the body
        var otpMatch = System.Text.RegularExpressions.Regex.Match(
            body, 
            @"class=""otp""[^>]*>(\d{6})</div>");
        
        var otp = otpMatch.Success ? otpMatch.Groups[1].Value : "N/A";
        
        _logger.LogInformation("========================================");
        _logger.LogInformation("📧 Sending email to: {Email}", toEmail);
        _logger.LogInformation("📝 Subject: {Subject}", subject);
        _logger.LogInformation("🔑 OTP: {Otp}", otp);
        _logger.LogInformation("========================================");
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_options.SmtpServer, _options.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_options.SmtpUsername, _options.SmtpPassword);
            await client.SendAsync(message);
            
            _logger.LogInformation("✅ Email sent successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send email via {SmtpServer}", _options.SmtpServer);
            _logger.LogWarning("💡 OTP is available in logs above for testing");
            // Swallowing or logging rather than throwing is standard for notification features if we want system resilience,
            // but we can rethrow to let the caller handle it.
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
