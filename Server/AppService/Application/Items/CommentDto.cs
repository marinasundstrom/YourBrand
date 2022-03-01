using Skynet.Application.Users;

namespace Skynet.Application.Items;

public record CommentDto(
    string Id, string? Text,
    DateTime Created, UserDto CreatedBy, DateTime? LastModified, UserDto? LastModifiedBy);