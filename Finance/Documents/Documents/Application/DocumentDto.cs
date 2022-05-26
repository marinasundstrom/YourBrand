namespace Documents.Application;

public record DocumentDto(string Id, string Title, string Extension, string ContentType, string Url, DateTime Created, DateTime? LastModified);