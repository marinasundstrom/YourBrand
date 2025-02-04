using System.Net.Http.Headers;

namespace YourBrand.TimeReport;

public class AuthForwardHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Split(" ").Last());
        }

        return base.SendAsync(request, cancellationToken);
    }
}