using MediatR;
using ChatApp.Domain;

namespace ChatApp.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    //where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Starting request {@RequestName}, {@DateTimeUtc}",
            typeof(TRequest).Name,
            DateTimeOffset.UtcNow);

        var response = await next();

        if (response is Result result && result.IsFailure)
        {
            logger.LogError(
            "Request failure {@RequestName}, {@Error} {@DateTimeUtc}",
            typeof(TRequest).Name,
            result.GetError(),
            DateTimeOffset.UtcNow);
        }

        logger.LogInformation(
                    "Completed request {@RequestName}, {@DateTimeUtc}",
                    typeof(TRequest).Name,
                    DateTimeOffset.UtcNow);

        return response;
    }
}