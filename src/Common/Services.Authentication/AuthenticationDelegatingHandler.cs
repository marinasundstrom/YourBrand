using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using YourBrand.Tenancy;

namespace YourBrand.Services.Authentication;

/// <summary>
/// Checks for access token (from user), or uses client credentials.
/// </summary>
public class AuthenticationDelegatingHandler(
    IHttpContextAccessor httpContextAccessor,
    ITokenProvider tokenProvider,
    ISettableTenantContext tenantContext,
    ILogger<AuthenticationDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = httpContextAccessor.HttpContext.User.FindFirst("access_token")?.Value;

        logger.LogInformation("Access token passed: {AccessToken}", accessToken);

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        return await GetClientAuthToken(tokenProvider, tenantContext, logger, request, cancellationToken);
    }

    private async Task<HttpResponseMessage> GetClientAuthToken(ITokenProvider tokenProvider, ISettableTenantContext tenantContext, ILogger<AuthenticationDelegatingHandler> logger, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var baseUrl = request.RequestUri!.GetLeftPart(UriPartial.Authority);

        var accessToken = await tokenProvider.RequestTokenAsync(baseUrl);

        if (accessToken is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            accessToken = await tokenProvider.RequestTokenAsync(baseUrl, false);

            logger.LogInformation("Retrieved new access token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        var stoken = ConvertJwtStringToJwtSecurityToken(accessToken);

        logger.LogInformation("Claims: " + string.Join(", ", stoken?.Claims.Select(x => x.Type)));

        tenantContext.SetTenantId(stoken?.Claims?.FirstOrDefault(x => x.Type == "client_tenant_id")?.Value!);

        return response;
    }

    public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        return token;
    }
}
