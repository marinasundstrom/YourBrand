namespace YourBrand.Sales;

public static class Results
{
    public readonly static Result Success = new();
}

public class Result
{
    private readonly Error? error;

    internal Result()
    {
    }

    protected Result(Error? error)
    {
        this.error = error;
    }

    public static Result Success() => new();

    public static Result Failure(Error error) => new(error);

    public static Result<T> Success<T>(T data) => new(data);

    public static Result<T> Failure<T>(Error error) => new(error);

    public bool IsSuccess => error is null;

    public bool IsFailure => error is not null;

    public bool HasError(Error error) => IsFailure && this.error == error;

    public bool HasError<T>() where T : Error => this.error is T;

    public Error? GetError() => error;

    public T? GetError<T>() where T : Error => (T?)error;

    public static implicit operator Error(Result result) =>
        !result.IsFailure
        ? throw new InvalidOperationException() : result.error!;


    public static implicit operator Result(Error error) =>
        Result.Failure(error);
}

public class Result<T> : Result
{
    private T? data { get; }

    public Result(T Data) : base(null)
    {
        data = Data;
    }

    public Result(Error error) : base(error: error)
    {
    }

    public T GetValue() => data!;

    public static implicit operator T(Result<T> result) =>
        result.IsFailure
        ? throw new InvalidOperationException() : result.data!;

    public static implicit operator Result<T>(T result) =>
        Result.Success(result);

    public static implicit operator Result<T>(Error error) =>
        Result.Failure<T>(error);
}