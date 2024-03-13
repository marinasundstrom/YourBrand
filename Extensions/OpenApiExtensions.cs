using Asp.Versioning;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Schema;

using NJsonSchema;
using NJsonSchema.Generation;

using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace YourBrand.Extensions;

public static class OpenApiExtensions
{
    readonly static IEnumerable<ApiVersion> apiVersionDescriptions = [
        new(1, 0),
        new(2, 0)
    ];

    public static IServiceCollection AddOpenApi(this IServiceCollection services, string documentTitle, IEnumerable<ApiVersion>? apiVersions = null)
    {
        services.AddEndpointsApiExplorer();

        apiVersions ??= apiVersionDescriptions;

        foreach (var apiVersion in apiVersions)
        {
            services.AddOpenApiDocument(config =>
            {
                config.DocumentName = $"v{GetApiVersion(apiVersion)}";
                config.PostProcess = document =>
                {
                    document.Info.Title = documentTitle;
                    document.Info.Version = $"v{GetApiVersion(apiVersion)}";
                };
                config.ApiGroupNames = [GetApiVersion(apiVersion)];

                config.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                config.SchemaSettings.SchemaNameGenerator = new CustomSchemaNameGenerator();

                config.AddSecurity("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                //config.OperationProcessors.Add(new TenantIdHeaderOperationProcessor(isRequired: false));
            });
        }

        static string GetApiVersion(ApiVersion apiVersion)
        {
            return (apiVersion.MinorVersion == 0
                ? apiVersion.MajorVersion.ToString()
                : apiVersion.ToString())!;
        }

        return services;
    }

    public static WebApplication UseOpenApi(this WebApplication app)
    {
        app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml");
        app.UseSwaggerUi(options =>
        {
            var descriptions = app.DescribeApiVersions();

            // build a swagger endpoint for each discovered API version
            foreach (var description in descriptions)
            {
                var name = $"v{description.ApiVersion}";
                var url = $"/swagger/v{GetApiVersion(description)}/swagger.yaml";

                options.SwaggerRoutes.Add(new SwaggerUiRoute(name, url));
            }
        });

        static string GetApiVersion(Asp.Versioning.ApiExplorer.ApiVersionDescription description)
        {
            var apiVersion = description.ApiVersion;
            return (apiVersion.MinorVersion == 0
                ? apiVersion.MajorVersion.ToString()
                : apiVersion.ToString())!;
        }

        return app;
    }

    public class CustomSchemaNameGenerator : ISchemaNameGenerator
    {
        public string Generate(Type type)
        {
            if (type.IsGenericType)
            {
                return $"{type.Name.Replace("`1", string.Empty)}Of{GenerateName(type.GetGenericArguments().First())}";
            }
            return GenerateName(type);
        }

        private static string GenerateName(Type type)
        {
            return type.Name
                .Replace("Dto", string.Empty)
                .Replace("Command", string.Empty)
                .Replace("Query", string.Empty);
        }
    }
}
