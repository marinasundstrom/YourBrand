namespace System;

public abstract record Result<TError>
{
    public sealed record Ok : Result<TError>;

    public sealed record Error(TError Err) : Result<TError>;
}

public abstract record ResultWithValue<T>
{
    public sealed record Ok(T Value) : ResultWithValue<T>;

    public sealed record Error() : ResultWithValue<T>;
}

public abstract record ResultWithValue<T, TError>
{
    public sealed record Ok(T Value) : ResultWithValue<T, TError>;

    public sealed record Error(TError Err) : ResultWithValue<T, TError>;
}
