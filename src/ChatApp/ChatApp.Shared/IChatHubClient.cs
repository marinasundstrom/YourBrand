namespace YourBrand.ChatApp;

public interface IChatHubClient
{
    Task OnMessagePosted(MessageData message);

    Task OnMessagePostedConfirmed(Guid messageId);

    Task OnMessageEdited(Guid channelId, MessageEditedData data);

    Task OnMessageDeleted(Guid channelId, MessageDeletedData data);

    Task OnMessageReaction(Guid channelId, Guid messageId, MessageReactionData reaction);

    Task OnMessageReactionRemoved(Guid channelId, Guid messageId, string reaction, string participantId);
}

public sealed record MessageData(Guid Id, Guid ChannelId, ReplyMessageData? ReplyTo, string Content, DateTimeOffset Posted, UserData PostedBy, DateTimeOffset? LastEdited, UserData? LastEditedBy, DateTimeOffset? Deleted, UserData? DeletedBy, IEnumerable<MessageReactionData> Reactions);

public sealed record ReplyMessageData(Guid Id, Guid ChannelId, string Content, DateTimeOffset Posted, UserData PostedBy, DateTimeOffset? LastEdited, UserData? LastEditedBy, DateTimeOffset? Deleted, UserData? DeletedBy);

public sealed record ReactionDto(string Content, DateTimeOffset Date, UserData User);

public sealed record MessageEditedData(Guid Id, DateTimeOffset LastEdited, UserData LastEditedBy, string Content);

public sealed record MessageDeletedData(Guid Id, bool HardDelete, DateTimeOffset? Deleted, UserData? DeletedBy);

public sealed record MessageReactionData(string Content, DateTimeOffset Date, UserData User, string ParticipantId);

public sealed record UserData(string Id, string Name);