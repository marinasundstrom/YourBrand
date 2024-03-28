using Microsoft.AspNetCore.Mvc;
using YourBrand.Ticketing.Application;
using YourBrand.Ticketing.Domain;

namespace YourBrand.Ticketing.Application;

public static class ControllerExtensions
{
    public static ActionResult HandleResult(this ControllerBase controller, Result result) => result.Handle(
            onSuccess: () => controller.Ok(),
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
