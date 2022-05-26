using YourBrand.Documents.Domain.Entities;

namespace YourBrand.Documents.Application;

public static class Mappings
{
    public static DocumentDto ToDto(this Document document, string url)
    {
        return new DocumentDto(document.Id, document.Name, document.Extension, document.ContentType, url, document.Created, document.LastModified);
    }
}