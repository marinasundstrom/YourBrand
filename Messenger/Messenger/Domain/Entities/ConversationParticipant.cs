
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Entities;

public class ConversationParticipant : AuditableEntity, ISoftDelete, IHasDomainEvent
{
    public string Id { get; set; } = null!;

    public Conversation Conversation { get; set; } = null!;

    public User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}