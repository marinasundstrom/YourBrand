using YourBrand.Documents.Domain.Entities;

namespace YourBrand.Documents.Application.Queries;

public class UrlResolver(IHttpContextAccessor httpContextAccessor) : IUrlResolver
{
    public string GetUrl(Document document)
    {
        var request = httpContextAccessor.HttpContext!.Request;

        return $"{request.Scheme}://{request.Host}/api/documents/documents/{document.Id}/File";

        //return $"{request.Scheme}://{request.Host}/content/documents/{blobId}";
    }
}