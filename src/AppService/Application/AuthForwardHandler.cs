using System.Net.Http.Headers;

using YourBrand.Identity;

namespace YourBrand.Application;

public class AuthForwardHandler(IUserContext userContext) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", userContext.GetAccessToken());

        return base.SendAsync(request, cancellationToken);
    }
}