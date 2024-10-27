using System.ComponentModel;
using System.Text;

using Asp.Versioning.ApiExplorer;

using Grpc.Core;

using MassTransit.Internals;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.Hosting;

internal static class OpenApiOptionsExtensions
{
    public static OpenApiOptions ApplyApiVersionInfo(this OpenApiOptions options, string title, string description)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            var versionedDescriptionProvider = context.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            var apiDescription = versionedDescriptionProvider?.ApiVersionDescriptions
                .SingleOrDefault(description => description.GroupName == context.DocumentName);
            if (apiDescription is null)
            {
                return Task.CompletedTask;
            }
            document.Info.Version = apiDescription.ApiVersion.ToString();
            document.Info.Title = title;
            document.Info.Description = BuildDescription(apiDescription, description);
            return Task.CompletedTask;
        });
        return options;
    }

    private static string BuildDescription(ApiVersionDescription api, string description)
    {
        var text = new StringBuilder(description);

        if (api.IsDeprecated)
        {
            if (text.Length > 0)
            {
                if (text[^1] != '.')
                {
                    text.Append('.');
                }

                text.Append(' ');
            }

            text.Append("This API version has been deprecated.");
        }

        if (api.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                if (text.Length > 0)
                {
                    text.Append(' ');
                }

                text.Append("The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                var rendered = false;

                foreach (var link in policy.Links.Where(l => l.Type == "text/html"))
                {
                    if (!rendered)
                    {
                        text.Append("<h4>Links</h4><ul>");
                        rendered = true;
                    }

                    text.Append("<li><a href=\"");
                    text.Append(link.LinkTarget.OriginalString);
                    text.Append("\">");
                    text.Append(
                        StringSegment.IsNullOrEmpty(link.Title)
                        ? link.LinkTarget.OriginalString
                        : link.Title.ToString());
                    text.Append("</a></li>");
                }

                if (rendered)
                {
                    text.Append("</ul>");
                }
            }
        }

        return text.ToString();
    }

    public static OpenApiOptions ApplySecuritySchemeDefinitions(this OpenApiOptions options)
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

        //options.AddDocumentTransformer<SecuritySchemeDefinitionsTransformer>();
        return options;
    }

    public static OpenApiOptions ApplyVersionTransforms(this OpenApiOptions options, string version)
    {
        const string VersionTemplate = "v{version}";

        // Rewrite paths and remove version placeholder

        options.AddDocumentTransformer((doc, context, ct) =>
        {
            foreach (var (path, item) in doc.Paths.ToList())
            {
                var oldPath = path;
                var newPath = path.Replace(VersionTemplate, version);

                doc.Paths.Add(newPath, item);
                doc.Paths.Remove(oldPath);
            }
            return Task.CompletedTask;
        });

        const string VersionParameterName = "version";

        // Remove version parameter

        options.AddOperationTransformer((operation, context, ct) =>
        {
            var versionParameter = operation.Parameters.FirstOrDefault(x => x.Name == VersionParameterName);
            if (versionParameter is not null)
            {
                operation.Parameters.Remove(versionParameter);
            }
            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions ApplyAuthorizationChecks(this OpenApiOptions options, string[] scopes)
    {
        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            if (!metadata.OfType<IAuthorizeData>().Any())
            {
                return Task.CompletedTask;
            }

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [oAuthScheme] = scopes
                }
            };

            return Task.CompletedTask;
        });
        return options;
    }

    public static OpenApiOptions ApplyOperationDeprecatedStatus(this OpenApiOptions options)
    {
        options.AddOperationTransformer(static (operation, context, cancellationToken) =>
        {
            var apiDescription = context.Description;
            operation.Deprecated |= apiDescription.IsDeprecated();
            return Task.CompletedTask;
        });
        return options;
    }

    public static OpenApiOptions ApplySchemaNameTransforms(this OpenApiOptions options)
    {
        options.AddOperationTransformer<NullabilityTransformer>();

        options.AddSchemaTransformer(static (schema, context, ct) =>
        {
            const string SchemaId = "x-schema-id";

            if (schema.Annotations?.TryGetValue(SchemaId, out var referenceIdObject) == true
                 && referenceIdObject is string newSchemaId)
            {
                var clrType = context.JsonTypeInfo.Type;
                newSchemaId = clrType.GetSchemaName();
                schema.Annotations[SchemaId] = newSchemaId;

                schema.Title = newSchemaId;
                schema.Description = clrType.GetAttribute<DescriptionAttribute>().FirstOrDefault()?.Description;

                // Schemas should never be nullable
                schema.Nullable = false;
            }

            return Task.CompletedTask;
        });

        return options;
    }

    private static IOpenApiAny? CreateOpenApiAnyFromObject(object value)
    {
        return value switch
        {
            bool b => new OpenApiBoolean(b),
            int i => new OpenApiInteger(i),
            double d => new OpenApiDouble(d),
            string s => new OpenApiString(s),
            _ => null
        };
    }

    private class SecuritySchemeDefinitionsTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var identitySection = configuration.GetSection("Identity");
            if (!identitySection.Exists())
            {
                return Task.CompletedTask;
            }

            var identityUrlExternal = identitySection.GetRequiredValue("Url");
            var scopes = identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value);
            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    // TODO: Change this to use Authorization Code flow with PKCE
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                        TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                        Scopes = scopes,
                    }
                }
            };
            document.Components ??= new();
            document.Components.SecuritySchemes.Add("oauth2", securityScheme);
            return Task.CompletedTask;
        }
    }
}

public static class Extensions2
{
    public static bool IsTypeNullable(this Type type)
    {
        return true;
    }

    public static Type UnwrapResultTypes(this Type type)
    {
        if (typeof(ActionResult<>).IsAssignableFrom(type))
        {
            return type.GetGenericArguments()[0];
        }
        return type;
    }

    public static Type UnwrapTaskTypes(this Type type)
    {
        if (typeof(Task).IsAssignableFrom(type) || typeof(ValueTask).IsAssignableFrom(type))
        {
            if (type.IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }
        }
        return type;
    }
}