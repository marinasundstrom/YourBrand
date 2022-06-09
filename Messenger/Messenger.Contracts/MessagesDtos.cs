namespace YourBrand.Messenger.Contracts;

public record ConversationDto(string Id, string? Title, IEnumerable<ParticipantDto>? Participants, UserDto CreatedBy, DateTime Created);

public record MessageDto(
    string Id, string ConversationId, string? Text,
    MessageDto? ReplyTo,
    IEnumerable<ReceiptDto>? Receipts,
    IEnumerable<MessageDto>? Replies,
    DateTime Sent, UserDto SentBy, DateTime? LastModified, UserDto? LastModifiedBy, DateTime? Deleted, UserDto? DeletedBy);

public record ReceiptDto(string Id, string MessageId, UserDto User, DateTime Date);

public record ParticipantDto(string Id, UserDto User, DateTime? Created);