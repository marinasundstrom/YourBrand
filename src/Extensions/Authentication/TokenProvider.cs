using System.Net;
using System.Net.Http.Headers;

using Azure.Core;

namespace YourBrand.Authentication;

public interface ITokenProvider
{
    Task<string?> RequestTokenAsync(string baseUrl, bool cached = true, CancellationToken cancellationToken = default);
}
