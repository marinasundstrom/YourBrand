
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Entities;

public class Message : AuditableEntity, ISoftDelete, IHasDomainEvent
{
    protected Message()
    {

    }

    public Message(string text, string? replyToId)
    {
        Id = Guid.NewGuid().ToString();
        Text = text;
        ReplyToId = replyToId;
    }

    public string Id { get; private set; } = null!;

    public string ConversationId { get; set; }

    public Conversation Conversation { get; set; }

    public string Text { get; set; } = null!;

    public List<MessageReceipt> Receipts { get; set; } = new List<MessageReceipt>();

    public Message? ReplyTo { get; set; }
    public string? ReplyToId { get; set; }
    public List<Message> Replies = new List<Message>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}