namespace RealEstateMarketplace.Application.Common;

public sealed record AuthError(string Code, string Message)
{
    public static AuthError InvalidCredentials() =>
        new("invalid_credentials", "Invalid email or password.");

    public static AuthError UserNotFound(string email) =>
        new("user_not_found", $"User '{email}' was not found.");

    public static AuthError EmailAlreadyExists(string email) =>
        new("email_already_exists", $"Email '{email}' is already registered.");

    public static AuthError InvalidResetToken() =>
        new("invalid_reset_token", "This password reset link is invalid or has expired.");

    public static AuthError PasswordResetFailed(string message) =>
        new("password_reset_failed", message);
}
