using YourBrand.Documents.Domain.Entities;

namespace YourBrand.Documents.Application.Queries;

public class UrlResolver : IUrlResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUrl(Document document)
    {
        var request = _httpContextAccessor.HttpContext!.Request;

        return $"{request.Scheme}://{request.Host}/api/documents/documents/{document.Id}/File";

        //return $"{request.Scheme}://{request.Host}/content/documents/{blobId}";
    }
}