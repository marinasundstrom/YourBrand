using System.Net;
using System.Net.Http.Headers;

using Microsoft.Extensions.Logging;

namespace YourBrand.Authentication;

public class AuthenticationDelegatingHandler(ITokenProvider tokenProvider, ILogger<AuthenticationDelegatingHandler> logger) : DelegatingHandler
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

        return response;
    }
}