using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

using YourBrand.Tenancy;

public class AuthenticationDelegatingHandler(
    ITokenProvider tokenProvider, 
    ITenantService tenantService, 
    ILogger<AuthenticationDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
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

        Console.WriteLine("Claims: " + string.Join(", ", stoken?.Claims.Select(x => x.Type)));

        tenantService.SetTenantId(stoken?.Claims?.FirstOrDefault(x => x.Type == "client_tenant_id")?.Value!);

        return response;
    }

    public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        return token;
    }
}