namespace Catalog.Application.Items;

public record CommentDto(
    string Id, string? Text,
    DateTime Created, string CreatedBy, DateTime? LastModified, string? LastModifiedBy);