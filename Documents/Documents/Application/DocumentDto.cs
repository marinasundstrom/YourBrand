namespace YourBrand.Documents.Application;

public record DocumentDto(string Id, string Name, string Extension, string ContentType, string? Description, string Url, DateTime Created, DateTime? LastModified);