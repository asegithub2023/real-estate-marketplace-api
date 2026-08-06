namespace RealEstateMarketplace.Application.Common;

public static class Result
{
    public static Result<TSuccess, TError> Success<TSuccess, TError>(TSuccess value) =>
        Result<TSuccess, TError>.Success(value);

    public static Result<TSuccess, TError> Failure<TSuccess, TError>(TError error) =>
        Result<TSuccess, TError>.Failure(error);
}

public sealed record Result<TSuccess, TError>(bool IsSuccess, TSuccess? Value, TError? Error)
{
    public static Result<TSuccess, TError> Success(TSuccess value) => new(true, value, default);

    public static Result<TSuccess, TError> Failure(TError error) => new(false, default, error);
}
