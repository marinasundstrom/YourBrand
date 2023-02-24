using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Events;

public record MessagePostedEvent : DomainEvent
{
    public MessagePostedEvent(string conversationId, string messageId)
    {
        this.ConversationId = conversationId;
        MessageId = messageId;
    }

    public string ConversationId { get; }
    public string MessageId { get; }
}

public record MessageEditedEvent : DomainEvent
{
    public MessageEditedEvent(string conversationId, string messageId)
    {
        this.ConversationId = conversationId;
        MessageId = messageId;
    }

    public string ConversationId { get; }
    public string MessageId { get; }
}

public record MessageDeletedEvent : DomainEvent
{
    public MessageDeletedEvent(string conversationId, string messageId)
    {
        this.ConversationId = conversationId;
        MessageId = messageId;
    }

    public string ConversationId { get; }
    public string MessageId { get; }
}