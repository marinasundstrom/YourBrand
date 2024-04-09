using YourBrand.Showroom.Application.Common.Interfaces;
namespace YourBrand.Showroom.WebApi.Services;

public class UrlHelper(IHttpContextAccessor httpContextAccessor) : IUrlHelper
{
    public string GetHostUrl()
    {
        var request = httpContextAccessor.HttpContext!.Request;

        string host = $"{request.Scheme}://{request.Host}";

        return $"{host}";
    }

    public string? CreateImageUrl(string? id)
    {
        if (id is null) return null;

        var host = GetHostUrl();

        //return $"{host}/content/images/{id}";

        return $"https://localhost:5174/content/images/{id}";
    }
}