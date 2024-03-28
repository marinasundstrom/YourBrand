namespace System;

public abstract record Option<T> : IDisposable
{
    public sealed record Some(T Value) : Option<T>;

    public sealed record None : Option<T> 
    {
        internal None() {}
    }

    public T GetValueOrDefault()
    {
        switch (this)
        {
            case Some(T Value):
                return Value;
        }
        return default(T)!;
    }

    public void Dispose()
    {
        switch (this)
        {
            case Some(T Value) when Value is IDisposable v:
                v.Dispose();
                break;
        }
    }

    public static explicit operator T(Option<T> result)
    {
        switch (result)
        {
            case Some(T Value):
                return Value;

            default:
            case None:
                throw new Exception("Unhandled error in result");
        }
    }

    /*
    public static implicit operator T(Option<T> result)
    {
        switch (result)
        {
            case Some(T Value):
                return Value;

            default:
            case None:
                throw new Exception("Unhandled error in result");
        }
    }
    */
}