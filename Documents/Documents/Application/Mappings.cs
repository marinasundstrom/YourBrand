using YourBrand.Documents.Domain.Entities;

using Directory = YourBrand.Documents.Domain.Entities.Directory;

namespace YourBrand.Documents.Application;

public static class Mappings
{
    public static DirectoryDto ToDto(this Directory directory, Func<Document, string> toString)
    {
        return new DirectoryDto(directory.Id, directory.Name, directory.Description, directory.Directories.Select(x => x.ToDto(toString)), directory.Documents.Select(x => x.ToDto(toString)), directory.Created, directory.LastModified);
    }

    public static DocumentDto ToDto(this Document document, Func<Document, string> toString)
    {
        return new DocumentDto(document.Id, document.Name, document.Extension, document.ContentType, document.Description, toString(document), document.Created, document.LastModified);
    }


    public static string GetUrl(Document document)
    {
        return "sdasd";
    }
}