namespace YourBrand.Portal.Services;

public interface IAccessTokenProvider
{
    Task<string?> GetAccessTokenAsync();
}

