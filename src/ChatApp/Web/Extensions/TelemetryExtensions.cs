using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace ChatApp.Web.Extensions;

public static class TelemetryExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        // Define some important constants to initialize tracing with
        var serviceName = "ChatApp";
        var serviceVersion = "1.0.0";

        // Configure important OpenTelemetry settings, the console exporter, and instrumentation library
        services.AddOpenTelemetryTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddConsoleExporter()
                .AddZipkinExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                    o.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
                })
                .AddSource(serviceName)
                .AddSource("MassTransit")
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddSqlClientInstrumentation()
                .AddMassTransitInstrumentation()
                .Configure((provider, b) =>
                {
                    var connection = provider.GetRequiredService<IConnectionMultiplexer>();

                    b.AddRedisInstrumentation(connection);
                });
        });

        return services;
    }
}