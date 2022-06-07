using YourBrand.Documents.Domain.Entities;

using Directory = YourBrand.Documents.Domain.Entities.Directory;

namespace YourBrand.Documents.Application;

public static class Mappings
{
    public static DirectoryDto ToDto(this Directory directory)
    {
        return new DirectoryDto(directory.Id, directory.Name, directory.Description, directory.Directories.Select(x => x.ToDto()), directory.Documents.Select(x => x.ToDto(GetUrl(x))), directory.Created, directory.LastModified);
    }

    public static DocumentDto ToDto(this Document document, string url)
    {
        return new DocumentDto(document.Id, document.Name, document.Extension, document.ContentType, document.Description, url, document.Created, document.LastModified);
    }


    public static string GetUrl(Document document)
    {
        return "sdasd";
    }
}