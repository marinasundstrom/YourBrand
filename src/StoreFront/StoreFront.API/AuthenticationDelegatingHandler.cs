using System.Net;
using System.Net.Http.Headers;

using Azure.Core;

using IdentityModel.Client;

using Microsoft.Identity.Client;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<AuthenticationDelegatingHandler> _logger;

    public AuthenticationDelegatingHandler(ITokenProvider tokenProvider, ILogger<AuthenticationDelegatingHandler> logger)
    {
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var baseUrl = request.RequestUri!.GetLeftPart(UriPartial.Authority);

        var accessToken = await _tokenProvider.RequestTokenAsync(baseUrl);

        if (accessToken is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            accessToken = await _tokenProvider.RequestTokenAsync(baseUrl, false);

            _logger.LogInformation("Retrieved new access token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}