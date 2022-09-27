using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Events;

public class MessagePostedEvent : DomainEvent
{
    public MessagePostedEvent(string conversationId, string messageId)
    {
        this.ConversationId = conversationId;
        MessageId = messageId;
    }

    public string ConversationId { get; }
    public string MessageId { get; }
}

public class MessageEditedEvent : DomainEvent
{
    public MessageEditedEvent(string conversationId, string messageId)
    {
        this.ConversationId = conversationId;
        MessageId = messageId;
    }

    public string ConversationId { get; }
    public string MessageId { get; }
}

public class MessageDeletedEvent : DomainEvent
{
    public MessageDeletedEvent(string conversationId, string messageId)
    {
        this.ConversationId = conversationId;
        MessageId = messageId;
    }

    public string ConversationId { get; }
    public string MessageId { get; }
}