using Microsoft.AspNetCore.Mvc;

namespace YourBrand;

public static class ControllerExtensions
{
    public static ActionResult HandleResult(this ControllerBase controller, Result result) => result.Handle(
            onSuccess: controller.Ok,
            onError: error =>
            {
                if (error.Id.EndsWith("NotFound"))
                {
                    return controller.NotFound();
                }
                return controller.Problem(detail: error.Detail, title: error.Title, type: error.Id);
            });

    public static ActionResult HandleResult<T>(this ControllerBase controller, Result<T> result) => result.Handle(
            onSuccess: data => controller.Ok(data),
            onError: error =>
            {
                if (error.Id.EndsWith("NotFound"))
                {
                    return controller.NotFound();
                }
                return controller.Problem(detail: error.Detail, title: error.Title, type: error.Id);
            });
}

public static class ResultExtensions
{
    public static ActionResult Handle(this Result result, Func<ActionResult> onSuccess, Func<Error, ActionResult> onError) => result.IsFailure ? onError(result) : onSuccess();

    public static ActionResult Handle<T>(this Result<T> result, Func<T, ActionResult> onSuccess, Func<Error, ActionResult> onError) => result.IsFailure ? onError(result) : onSuccess(result);
}