using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace YourBrand;

public static class DiagnosticExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services,
        string serviceName,
        string serviceVersion,
        IConfiguration configuration,
        Action<TracerProviderBuilder>? tracerBuilderAction = null)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        var resource = ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion);

        services
            .AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics
                        .SetResourceBuilder(resource)
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEventCountersInstrumentation(c =>
                        {
                            c.AddEventSources(
                                "Microsoft.AspNetCore.Hosting",
                                "Microsoft-AspNetCore-Server-Kestrel",
                                "System.Net.Http",
                                "System.Net.Sockets");
                        })
                        .AddView("request-duration", new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                        })
                        .AddMeter("Microsoft.AspNetCore.Hosting",
                            "Microsoft.AspNetCore.Server.Kestrel")
                        .AddPrometheusExporter();
                    //.AddConsoleExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing.SetResourceBuilder(resource)
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation(o =>
                            {
                                o.RecordException = true;
                            })
                            .AddSqlClientInstrumentation()
                            .AddMassTransitInstrumentation()
                            .AddSource("MassTransit")
                            .AddSource("Yarp.ReverseProxy")
                            .AddRedisInstrumentation()
                            //.AddGrpcClientInstrumentation()
                            .AddZipkinExporter(zipkin =>
                            {
                                var zipkinUrl = configuration["ZIPKIN_URL"] ?? "http://localhost:9411";
                                zipkin.Endpoint = new Uri($"{zipkinUrl}/api/v2/spans");
                            });
                    //.AddConsoleExporter();

                    tracerBuilderAction?.Invoke(tracing);
                });

        return services;
    }

    public static void MapObservability(this WebApplication app)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}