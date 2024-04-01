using Asp.Versioning;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using NJsonSchema;
using NJsonSchema.Generation;

using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace YourBrand.Extensions;

public static class OpenApiExtensions
{
    readonly static IEnumerable<ApiVersion> apiVersionDescriptions = [
        new(1, 0),
        new(2, 0)
    ];

    public static IServiceCollection AddOpenApi(this IServiceCollection services,
        string documentTitle,
        IEnumerable<ApiVersion>? apiVersions = null,
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? setting = null)
    {
        services.AddEndpointsApiExplorer();

        apiVersions ??= apiVersionDescriptions;

        foreach (var apiVersion in apiVersions)
        {
            services.AddOpenApiDocument(settings =>
            {
                settings.Title = documentTitle;

                settings.DocumentName = $"v{GetApiVersion(apiVersion)}";
                settings.PostProcess = document =>
                {
                    document.Info.Title = documentTitle;
                    document.Info.Version = $"v{GetApiVersion(apiVersion)}";
                };
                settings.ApiGroupNames = [GetApiVersion(apiVersion)];

                settings.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                settings.SchemaSettings.SchemaNameGenerator = new CustomSchemaNameGenerator();

                setting?.Invoke(settings);
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

    public static AspNetCoreOpenApiDocumentGeneratorSettings AddJwtSecurity(this AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        settings.AddSecurity("JWT", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Type into the textbox: Bearer {your JWT token}."
        });

        settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

        return settings;
    }

    public static AspNetCoreOpenApiDocumentGeneratorSettings AddApiKeySecurity(this AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        settings.AddSecurity("ApiKey", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "X-API-Key",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Type into the textbox: {your API key}."
        });

        settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("ApiKey"));

        return settings;
    }

    public static AspNetCoreOpenApiDocumentGeneratorSettings RequireTenantId(this AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        settings.OperationProcessors.Add(new TenantIdHeaderOperationProcessor(isRequired: false));

        return settings;
    }

    public static WebApplication UseOpenApiAndSwaggerUi(
        this WebApplication app,
        Action<OpenApiDocumentMiddlewareSettings>? configureOpenApi = null,
        Action<SwaggerUiSettings>? configureSwaggerUi = null)
    {
        app.UseOpenApi(options =>
        {
            configureOpenApi?.Invoke(options);

            options.Path = "/swagger/{documentName}/swagger.yaml";
        });

        app.UseSwaggerUi(options =>
        {
            configureSwaggerUi?.Invoke(options);

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