
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Entities;

public class Message : AuditableEntity, ISoftDelete
{
    private readonly HashSet<MessageReceipt> _receipts = new HashSet<MessageReceipt>();
    private readonly HashSet<Message> _replies = new HashSet<Message>();

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

    public IReadOnlyCollection<MessageReceipt> Receipts => _receipts;

    public void AddReceipt(MessageReceipt receipt)
    {
        _receipts.Add(receipt);
    }

    public Message? ReplyTo { get; set; }
    public string? ReplyToId { get; set; }

    public IReadOnlyCollection<Message> Replies => _replies;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}