using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Scalar.AspNetCore;

using Serilog;

namespace Microsoft.Extensions.Hosting;

public static partial class Extensions
{
    public static IHostApplicationBuilder AddDefaultLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog(lc => lc
            .WriteTo.Console()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
                var headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];
                foreach (var header in headers)
                {
                    var (key, value) = header.Split('=') switch
                    {
                    [string k, string v] => (k, v),
                        var v => throw new Exception($"Invalid header format {v}")
                    };

                    options.Headers.Add(key, value);
                }
                //options.ResourceAttributes.Add("service.name", ServiceName);
            })
            //.Enrich.WithProperty("Application", ServiceName)
            //.Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
            .ReadFrom.Configuration(builder.Configuration));
            
        return builder;
    }
}
