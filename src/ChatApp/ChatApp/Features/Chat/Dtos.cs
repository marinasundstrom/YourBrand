using YourBrand.ChatApp.Features.Chat.Channels;
using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Features.Chat;

public sealed record MessageDto(Guid Id, Guid ChannelId, ReplyMessageDto? ReplyTo, string Content, DateTimeOffset Posted, ParticipantDto PostedBy, DateTimeOffset? LastEdited, ParticipantDto? LastEditedBy, DateTimeOffset? Deleted, ParticipantDto? DeletedBy, IEnumerable<ReactionDto> Reactions);

public sealed record ReplyMessageDto(Guid Id, Guid ChannelId, string Content, DateTimeOffset Posted, ParticipantDto PostedBy, DateTimeOffset? LastEdited, ParticipantDto? LastEditedBy, DateTimeOffset? Deleted, ParticipantDto? DeletedBy);

public sealed record ReactionDto(string Content, DateTimeOffset Date, ParticipantDto AddedBy);

public enum TodoStatusDto
{
    NotStarted,
    InProgress,
    OnHold,
    Completed
}