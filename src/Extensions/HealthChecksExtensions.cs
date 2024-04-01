using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Extensions;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddHealthChecksServices(this IServiceCollection services)
    {
        return services.AddHealthChecks();
    }

    public static IEndpointRouteBuilder MapHealthChecks(this IEndpointRouteBuilder builder)
    {
        builder.MapHealthChecks("/healthz", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return builder;
    }
}