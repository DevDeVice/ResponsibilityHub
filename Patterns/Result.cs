using System.Diagnostics;

namespace ResponsibilityHub.Patterns;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}
public class Result
{
    public bool _isSuccess { get; }
    public Error _error { get; }
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        _isSuccess = isSuccess;
        _error = error;
    }
    public static Result Success() => new(true, Error.None);
    public static Result Faulure(Error error) => new(false, error);
}
