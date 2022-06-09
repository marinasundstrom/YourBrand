using System.Security.Claims;

namespace YourBrand.Messenger.Contracts;

public record PostMessage(string AccessToken, string ConversationId, string Text, string? ReplyToId = null);

public record UpdateMessage(string AccessToken, string ConversationId, string MessageId, string Text);

public record DeleteMessage(string AccessToken, string ConversationId, string MessageId);

public record MarkMessageAsRead(string AccessToken, string ConversationId, string MessageId);


public record StartTyping(string AccessToken, string ConversationId, string MessageId);

public record EndTyping(string AccessToken, string ConversationId, string MessageId);


public record MessagePosted(MessageDto Message);

public record MessageUpdated(string ConversationId, string MessageId, string Text, DateTime Edited);

public record MessageDeleted(string ConversationId, string MessageId);

public record MessageRead(ReceiptDto Receipt /* string UserId, string ConversationId, string MessageId */);


public record StartedTyping(string UserId, string ConversationId, string MessageId);

public record EndedTyping(string UserId, string ConversationId, string MessageId);