// namespace System;

// public abstract record Result<T> : IDisposable
// {
//     public sealed record Ok(T Value) : Result<T>;

//     public sealed record Error() : Result<T>;

//     public T GetValueOrDefault()
//     {
//         switch (this)
//         {
//             case Ok(T Value):
//                 return Value;
//         }
//         return default(T)!;
//     }

//     public void Dispose()
//     {
//         switch (this)
//         {
//             case Ok(T Value) when Value is IDisposable v:
//                 v.Dispose();
//                 break;
//         }
//     }

//     public static implicit operator T(Result<T> result)
//     {
//         switch (result)
//         {
//             case Ok(T Value):
//                 return Value;

//             default:
//             case Error:
//                 throw new Exception("Unhandled error in result");
//         }
//     }
// }

// public abstract record Result<T, TError> : IDisposable
// {
//     public sealed record Ok(T Value) : Result<T, TError>;

//     public sealed record Error(TError Err) : Result<T, TError>;

//     public T GetValueOrDefault()
//     {
//         switch (this)
//         {
//             case Ok(T Value):
//                 return Value;
//         }
//         return default(T)!;
//     }

//     public void Dispose()
//     {
//         switch (this)
//         {
//             case Ok(T Value) when Value is IDisposable v:
//                 v.Dispose();
//                 break;
//         }
//     }

//     public static explicit operator T(Result<T, TError> result)
//     {
//         switch (result)
//         {
//             case Ok(T Value):
//                 return Value;

//             case Error(TError Err):
//                 throw new Exception($"Unhandled error in result: {Err}");

//             default:
//                 throw new Exception("Unexpected exception");
//         }
//     }

//     /*
//     public static implicit operator T(Result<T, TError> result)
//     {
//         switch (result)
//         {
//             case Ok(T Value):
//                 return Value;

//             case Error(TError Err):
//                 throw new Exception($"Unhandled error in result: {Err}");

//             default:
//                 throw new Exception("Unexpected exception");
//         }
//     }
//     */
// }