using Asp.Versioning;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace ChatApp.Web.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var apiVersionDescriptions = new[] {
            (ApiVersion: new ApiVersion(1, 0), foo: 1),
            (ApiVersion: new ApiVersion(2, 0), foo: 1)
        };

        foreach (var description in apiVersionDescriptions)
        {
            services.AddOpenApiDocument(config =>
            {
                config.DocumentName = $"v{GetApiVersion(description)}";
                config.PostProcess = document =>
                {
                    document.Info.Title = "Chat API";
                    document.Info.Version = $"v{GetApiVersion(description)}";
                };
                config.ApiGroupNames = new[] { GetApiVersion(description) };

                config.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;

                config.AddSecurity("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

                config.SchemaNameGenerator = new CustomSchemaNameGenerator();
            });
        }

        return services;
    }

    private static string GetApiVersion((ApiVersion ApiVersion, int foo) description)
    {
        var apiVersion = description.ApiVersion;
        return (apiVersion.MinorVersion == 0
            ? apiVersion.MajorVersion.ToString()
            : apiVersion.ToString())!;
    }
}
