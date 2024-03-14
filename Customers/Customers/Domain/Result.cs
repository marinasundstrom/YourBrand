namespace YourBrand.Customers.Domain;

public class Result
{
    private readonly Error? error;

    private Result()
    {
    }

    protected Result(Error? error)
    {
        this.error = error;
    }

    public static Result Success() => new Result();

    public static Result Failure(Error error) => new Result(error);

    public static Result<T> Success<T>(T data) => new(data);

    public static Result<T> Failure<T>(Error error) => new(error);

    public bool IsSuccess => error is null;

    public bool IsFailure => error is not null;

    public bool HasError(Error error) => IsFailure && this.error == error;

    public bool HasError<T>(T error) where T : Error => this.error is T;

    public Error? GetError() => error;

    public T? GetError<T>() where T : Error => (T?)error;

    public static implicit operator Error(Result result) =>
        !result.IsFailure
        ? throw new InvalidOperationException() : result.error!;
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
}
