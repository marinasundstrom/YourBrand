using Catalog.Domain.Common;

namespace Catalog.Domain.Events;

public class CommentPostedEvent : DomainEvent
{
    public CommentPostedEvent(string itemId, string commentId)
    {
        this.ItemId = itemId;
        CommentId = commentId;
    }

    public string ItemId { get; }

    public string CommentId { get; }
}