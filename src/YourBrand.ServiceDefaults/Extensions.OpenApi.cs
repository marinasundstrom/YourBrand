using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

using MartinCostello.OpenApi;
using MartinCostello.OpenApi.Transformers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using YourBrand.Extensions;

using Scalar.AspNetCore;

namespace Microsoft.Extensions.Hosting;

public static partial class Extensions
{
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        /*
        var configuration = app.Configuration;
        var openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists())
        {
            return app;
        }
        */

        //app.MapOpenApiYaml();
        app.UseOpenApiAndSwaggerUi();

        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference(x =>
            {
                x.Title = "Web API";
                x.OpenApiRoutePattern = "/openapi/{documentName}.yaml";
            });
            app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
        }

        return app;
    }

    public static IHostApplicationBuilder AddDefaultOpenApi(
        this IHostApplicationBuilder builder,
        IApiVersioningBuilder? apiVersioning = default)
    {
        var openApi = builder.Configuration.GetSection("OpenApi");

        var name = openApi.GetRequiredValue("Document:Title");
        var description = openApi.GetRequiredValue("Document:Description");

        builder.Services
            .AddOpenApi(name, description, null, settings => settings.AddJwtSecurity())
            .AddApiVersioningServices();
            
        //NewMethod(apiVersioning);

        return builder;
    }

    private static void MsOpenApi(
        IHostApplicationBuilder builder, IApiVersioningBuilder? apiVersioning)
    {
        var openApi = builder.Configuration.GetSection("OpenApi");
        var identitySection = builder.Configuration.GetSection("Identity");

        /*
        var scopes = identitySection.Exists()
            ? identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value)
            : new Dictionary<string, string?>();


        if (!openApi.Exists())
        {
            return builder;
        }
        */

        if (apiVersioning is not null)
        {
            // the default format will just be ApiVersion.ToString(); for example, 1.0.
            // this will format the version as "'v'major[.minor][-status]"
            var versioned = apiVersioning.AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            string[] versions = ["v1"];
            foreach (var description in versions)
            {
                builder.Services.AddOpenApi(description, options =>
                {
                    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;

                    options.ApplyApiVersionInfo(openApi.GetRequiredValue("Document:Title"), openApi.GetRequiredValue("Document:Description"));
                    options.AddDocumentTransformer<AddServersTransformer>();
                    options.ApplyVersionTransforms(description);
                    options.ApplySchemaNameTransforms();
                    options.ApplyAuthorizationChecks(["myapi"]); //([.. scopes.Keys]);
                    options.ApplySecuritySchemeDefinitions();
                    options.ApplyOperationDeprecatedStatus();
                });
            }
        }
    }
}
