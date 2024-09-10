namespace YourBrand.ChatApp;

public interface IChatHubClient
{
    Task OnMessagePosted(MessageData message);

    Task OnMessagePostedConfirmed(string messageId);

    Task OnMessageEdited(string channelId, MessageEditedData data);

    Task OnMessageDeleted(string channelId, MessageDeletedData data);

    Task OnMessageReaction(string channelId, string messageId, MessageReactionData reaction);

    Task OnMessageReactionRemoved(string channelId, string messageId, string reaction, string participantId);
}

public sealed record MessageData(string Id, string ChannelId, ReplyMessageData? ReplyTo, string Content, DateTimeOffset Posted, ParticipantData PostedBy, DateTimeOffset? LastEdited, ParticipantData? LastEditedBy, DateTimeOffset? Deleted, ParticipantData? DeletedBy, IEnumerable<MessageReactionData> Reactions);

public sealed record ReplyMessageData(string Id, string ChannelId, string Content, DateTimeOffset Posted, ParticipantData PostedBy, DateTimeOffset? LastEdited, ParticipantData? LastEditedBy, DateTimeOffset? Deleted, ParticipantData? DeletedBy);

public sealed record ReactionDto(string Content, DateTimeOffset Date, ParticipantData User);

public sealed record MessageEditedData(string Id, DateTimeOffset LastEdited, ParticipantData LastEditedBy, string Content);

public sealed record MessageDeletedData(string Id, bool HardDelete, DateTimeOffset? Deleted, ParticipantData? DeletedBy);

public sealed record MessageReactionData(string Content, DateTimeOffset Date, ParticipantData AddedBy);

public sealed record UserData(string Id, string Name);

public sealed record ParticipantData(string Id, string Name, string? UserId);