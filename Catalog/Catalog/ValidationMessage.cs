using FluentValidation;

namespace YourBrand.Catalog;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var validator = ctx.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is not null)
        {
            var entity = ctx.Arguments
              .OfType<T>()
              .FirstOrDefault(a => a?.GetType() == typeof(T));
            if (entity is not null)
            {
                var validation = await validator.ValidateAsync(entity);
                if (validation.IsValid)
                {
                    return await next(ctx);
                }
                return Results.ValidationProblem(validation.ToDictionary());
            }
            else
            {
                return Results.Problem("Could not find type to validate");
            }
        }
        return await next(ctx);
    }
}