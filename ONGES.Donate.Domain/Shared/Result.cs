namespace ONGES.Donate.Domain.Shared;

public class Result
{
    protected Result(bool isSuccess, Error? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error? Error { get; }

    public static Result Success() => new(true);
    public static Result Failure(Error error) => new(false, error);
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value = default, Error? error = null) : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value);
    public static Result<T> Failure(Error error) => new(false, default, error);
}
