using YourBrand.Application.Users;

namespace YourBrand.Application.Items;

public record CommentDto(
    string Id, string? Text,
    DateTime Created, UserDto CreatedBy, DateTime? LastModified, UserDto? LastModifiedBy);