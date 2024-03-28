namespace YourBrand.Documents.Application;

public record DirectoryDto(string Id, string Name, string? Description, IEnumerable<DirectoryDto> Directories, IEnumerable<DocumentDto> Documents, DateTime Created, DateTime? LastModified);

public record DocumentDto(string Id, string Name, string Extension, string ContentType, string? Description, string Url, DateTime Created, DateTime? LastModified);