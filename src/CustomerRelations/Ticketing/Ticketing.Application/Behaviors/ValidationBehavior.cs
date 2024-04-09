using FluentValidation;

using MediatR;

using ValidationException = YourBrand.Ticketing.Application.Exceptions.ValidationException;

namespace YourBrand.Ticketing.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errorsDictionary = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);

        if (errorsDictionary.Any())
        {
            throw new ValidationException(errorsDictionary);
        }

        return await next();
    }
}