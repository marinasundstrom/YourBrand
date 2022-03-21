using YourCompany.Application.Users;

namespace YourCompany.Application.Items;

public record CommentDto(
    string Id, string? Text,
    DateTime Created, UserDto CreatedBy, DateTime? LastModified, UserDto? LastModifiedBy);