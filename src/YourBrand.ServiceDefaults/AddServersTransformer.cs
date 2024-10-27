using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace MartinCostello.OpenApi.Transformers;

/// <summary>
/// A class that server information to an OpenAPI document. This class cannot be inherited.
/// </summary>
/// <param name="extensionsOptions"> The configured <see cref="OpenApiExtensionsOptions"/>.</param>
/// <param name="accessor">The <see cref="IHttpContextAccessor"/> to use, if available.</param>
/// <param name="forwardedHeadersOptions">The configured <see cref="ForwardedHeadersOptions"/>, if any.</param>
internal sealed class AddServersTransformer(
    IHttpContextAccessor? accessor,
    IOptions<ForwardedHeadersOptions>? forwardedHeadersOptions) : IOpenApiDocumentTransformer
{
    /// <inheritdoc/>
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (GetServerUrl() is { } url)
        {
            document.Servers = [new() { Url = url }];
        }

        return Task.CompletedTask;
    }

    private string? GetServerUrl()
    {
        if (accessor?.HttpContext?.Request is not { } request)
        {
            return "";
        }

        if (forwardedHeadersOptions?.Value is not { } options)
        {
            return null;
        }

        var scheme = TryGetFirstHeader(options.ForwardedProtoHeaderName) ?? request.Scheme;
        var host = TryGetFirstHeader(options.ForwardedHostHeaderName) ?? request.Host.ToString();

        return new Uri($"{scheme}://{host}").ToString().TrimEnd('/');

        string? TryGetFirstHeader(string name)
            => request.Headers.TryGetValue(name, out var values) ? values.FirstOrDefault() : null;
    }
}