using ChatApp.Domain;

namespace ChatApp.Extensions;

public static class ResultExtensions
{
    public static IResult Handle(this Result result, Func<IResult> onSuccess, Func<Error, IResult> onError) => result.IsFailure ? onError(result) : onSuccess();

    public static IResult Handle<T>(this Result<T> result, Func<T, IResult> onSuccess, Func<Error, IResult> onError) => result.IsFailure ? onError(result) : onSuccess(result);
}