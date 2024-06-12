using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace ChatApp.Web.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.OnRejected = (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                // context.Lease.GetAllMetadata().ToList()
                //    .ForEach(m => app.Logger.LogWarning($"Rate limit exceeded: {m.Key} {m.Value}"));

                return new ValueTask();
            };

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = 5;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 2;
                options.Window = TimeSpan.FromSeconds(2);
                options.AutoReplenishment = false;
            });
        });

        return services;
    }
}
