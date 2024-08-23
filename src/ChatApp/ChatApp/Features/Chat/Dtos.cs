using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Features.Chat;

public sealed record MessageDto(Guid Id, Guid ChannelId, ReplyMessageDto? ReplyTo, string Content, DateTimeOffset Posted, UserDto PostedBy, DateTimeOffset? LastEdited, UserDto? LastEditedBy, DateTimeOffset? Deleted, UserDto? DeletedBy, IEnumerable<ReactionDto> Reactions);

public sealed record ReplyMessageDto(Guid Id, Guid ChannelId, string Content, DateTimeOffset Posted, UserDto PostedBy, DateTimeOffset? LastEdited, UserDto? LastEditedBy, DateTimeOffset? Deleted, UserDto? DeletedBy);

public sealed record ReactionDto(string Content, DateTimeOffset Date, UserDto User, string ParticipantId);

public enum TodoStatusDto
{
    NotStarted,
    InProgress,
    OnHold,
    Completed
}