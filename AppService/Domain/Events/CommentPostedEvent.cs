using YourBrand.Domain.Common;

namespace YourBrand.Domain.Events;

public record CommentPostedEvent : DomainEvent
{
    public CommentPostedEvent(string itemId, string commentId)
    {
        this.ItemId = itemId;
        CommentId = commentId;
    }

    public string ItemId { get; }

    public string CommentId { get; }
}