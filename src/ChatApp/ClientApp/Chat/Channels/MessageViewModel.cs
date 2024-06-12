namespace ChatApp.Chat.Channels;

public class MessageViewModel : IComparable<MessageViewModel>
{
    public Guid Id { get; set; }

    public DateTimeOffset Published { get; set; }
    public string PostedById { get; set; } = default!;
    public string PostedByName { get; set; } = default!;
    public string PostedByInitials { get; set; } = default!;

    public DateTimeOffset? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public string? DeletedByName { get; set; }

    public DateTimeOffset? Edited { get; set; }
    public string? EditedById { get; set; }
    public string? EditedByName { get; set; }

    public MessageViewModel? ReplyTo { get; set; }

    public string Content { get; set; } = default!;

    public bool IsFromCurrentUser { get; set; } = default!;
    public bool Confirmed { get; set; }

    public bool IsReactionsVisible { get; set; } = false;

    public List<Reaction> Reactions { get; set; } = new List<Reaction>();

    public int CompareTo(MessageViewModel? other)
    {
        if (other is null) return 1;

        return this.Published.CompareTo(other.Published);
    }
}