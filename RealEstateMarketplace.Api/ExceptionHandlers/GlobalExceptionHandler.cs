using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateMarketplace.Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred.");

        var (statusCode, title, detail) = exception switch
        {
            InvalidOperationException invalidOperation when invalidOperation.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) =>
                (StatusCodes.Status409Conflict, "Conflict", invalidOperation.Message),
            InvalidOperationException invalidOperation when invalidOperation.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) =>
                (StatusCodes.Status404NotFound, "Resource not found", invalidOperation.Message),
            InvalidOperationException invalidOperation =>
                (StatusCodes.Status400BadRequest, "Bad request", invalidOperation.Message),
            _ =>
                (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "Please try again later or contact support if the issue persists.")
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
