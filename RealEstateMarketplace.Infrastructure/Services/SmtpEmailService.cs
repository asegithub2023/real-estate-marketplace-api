using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RealEstateMarketplace.Application.Interfaces.Services;

namespace RealEstateMarketplace.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(
        string toEmail,
        string toName,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var emailSettings = _configuration.GetSection("Email");

        var host = emailSettings["SmtpHost"];
        var port = emailSettings.GetValue<int>("SmtpPort");
        var username = emailSettings["SmtpUsername"];
        var password = emailSettings["SmtpPassword"];
        var fromEmail = emailSettings["FromEmail"] ?? username;
        var fromName = emailSettings["FromName"] ?? "Real Estate Marketplace";

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username))
        {

            _logger.LogWarning(
                "Email is not configured. Password reset link for {Email}: {ResetLink}",
                toEmail,
                resetLink);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail!, fromName),
            Subject = "Reset your password",
            Body =
                $"Hi {toName},\n\n" +
                "We received a request to reset your password. Click the link below to choose a new one:\n\n" +
                $"{resetLink}\n\n" +
                "If you didn't request this, you can safely ignore this email.\n\n" +
                "This link will expire soon for your security.",
            IsBodyHtml = false
        };

        message.To.Add(new MailAddress(toEmail, toName));

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,

            Timeout = 15000
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
