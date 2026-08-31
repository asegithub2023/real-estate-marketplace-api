namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string toEmail,
        string toName,
        string resetLink,
        CancellationToken cancellationToken = default);
}