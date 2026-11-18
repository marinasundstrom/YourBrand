namespace YourBrand.AspNetCore.Idempotency;

internal sealed class IdempotencyFilter(int cacheTimeInMinutes = 60)
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Parse the Idempotence-Key header from the request
        if (TryGetIdempotenceKey(out Guid idempotenceKey))
        {
            return Results.BadRequest("Invalid or missing Idempotence-Key header");
        }

        IDistributedCache cache = context.HttpContext
            .RequestServices.GetRequiredService<IDistributedCache>();

        // Check if we already processed this request and return a cached response (if it exists)
        string cacheKey = $"Idempotent_{idempotenceKey}";
        string? cachedResult = await cache.GetStringAsync(cacheKey);
        if (cachedResult is not null)
        {
            IdempotentResponse response = JsonSerializer.Deserialize<IdempotentResponse>(cachedResult)!;
            return new IdempotentResult(response.StatusCode, response.Value);
        }

        object? result = await next(context);

        // Execute the request and cache the response for the specified duration
        if (result is IStatusCodeHttpResult { StatusCode: >= 200 and < 300 } statusCodeResult
            and IValueHttpResult valueResult)
        {
            int statusCode = statusCodeResult.StatusCode ?? StatusCodes.Status200OK;
            IdempotentResponse response = new(statusCode, valueResult.Value);

            await cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(response),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes)
                }
            );
        }

        return result;
    }
}

// We have to implement a custom result to write the status code
internal sealed class IdempotentResult : IResult
{
    private readonly int _statusCode;
    private readonly object? _value;

    public IdempotentResult(int statusCode, object? value)
    {
        _statusCode = statusCode;
        _value = value;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = _statusCode;

        return httpContext.Response.WriteAsJsonAsync(_value);
    }
}
