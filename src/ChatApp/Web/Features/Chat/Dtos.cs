using ChatApp.Features.Users;

namespace ChatApp.Features.Chat;

public sealed record MessageDto(Guid Id, Guid ChannelId, ReplyMessageDto? ReplyTo, string Content, DateTimeOffset Published, UserDto PublishedBy, DateTimeOffset? LastEdited, UserDto? LastEditedBy, DateTimeOffset? Deleted, UserDto? DeletedBy, IEnumerable<ReactionDto> Reactions);

public sealed record ReplyMessageDto(Guid Id, Guid ChannelId, string Content, DateTimeOffset Published, UserDto PublishedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy, DateTimeOffset? Deleted, UserDto? DeletedBy);

public sealed record ReactionDto(string Content, DateTimeOffset Date, UserDto User);

public enum TodoStatusDto
{
    NotStarted,
    InProgress,
    OnHold,
    Completed
}
