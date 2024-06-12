namespace ChatApp;

public interface IChatHubClient
{
    Task OnMessagePosted(MessageData message);

    Task OnMessagePostedConfirmed(Guid messageId);

    Task OnMessageEdited(Guid channelId, MessageEditedData data);

    Task OnMessageDeleted(Guid channelId, MessageDeletedData data);

    Task OnMessageReaction(Guid channelId, Guid messageId, MessageReactionData reaction);

    Task OnMessageReactionRemoved(Guid channelId, Guid messageId, string reaction, string userId);
}

public sealed record MessageData(Guid Id, Guid ChannelId, ReplyMessageData? ReplyTo, string Content, DateTimeOffset Published, UserData PublishedBy, DateTimeOffset? LastEdited, UserData? LastEditedBy, DateTimeOffset? Deleted, UserData? DeletedBy, IEnumerable<MessageReactionData> Reactions);

public sealed record ReplyMessageData(Guid Id, Guid ChannelId, string Content, DateTimeOffset Published, UserData PublishedBy, DateTimeOffset? LastModified, UserData? LastModifiedBy, DateTimeOffset? Deleted, UserData? DeletedBy);

public sealed record ReactionDto(string Content, DateTimeOffset Date, UserData User);

public sealed record MessageEditedData(Guid Id, DateTimeOffset LastEdited, UserData LastEditedBy, string Content);

public sealed record MessageDeletedData(Guid Id, DateTimeOffset Deleted, UserData DeletedBy);

public sealed record MessageReactionData(string Content, DateTimeOffset Date, UserData User);

public sealed record UserData(string Id, string Name);
