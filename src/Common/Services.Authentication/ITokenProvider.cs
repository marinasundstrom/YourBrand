namespace YourBrand.Services.Authentication;

public interface ITokenProvider
{
    Task<string?> RequestTokenAsync(string baseUrl, bool cached = true, CancellationToken cancellationToken = default);
}