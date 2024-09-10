namespace YourBrand.ChatApp.Chat.Channels;

public class MessageViewModel : IComparable<MessageViewModel>
{
    public string Id { get; set; }

    public string ChannelId { get; set; }

    public DateTimeOffset Posted { get; set; }
    public string PostedById { get; set; } = default!;
    public string? PostedByUserId { get; set; }
    public string PostedByName { get; set; } = default!;
    public string PostedByInitials { get; set; } = default!;

    public DateTimeOffset? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public string? DeletedByName { get; set; }

    public DateTimeOffset? LastEdited { get; set; }
    public string? LastEditedById { get; set; }
    public string? LastEditedByName { get; set; }

    public MessageViewModel? ReplyTo { get; set; }

    public string Content { get; set; } = default!;

    public bool IsFromCurrentUser { get; set; } = default!;
    public bool Confirmed { get; set; }

    public bool IsReactionsVisible { get; set; } = false;

    public List<Reaction> Reactions { get; set; } = new List<Reaction>();

    public int CompareTo(MessageViewModel? other)
    {
        if (other is null) return 1;

        return this.Posted.CompareTo(other.Posted);
    }
}