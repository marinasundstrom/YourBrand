
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Entities;

public class Conversation : AuditableEntity, ISoftDelete, IHasDomainEvent
{
    public string Id { get; set; } = null!;

    public string? Title { get; set; }

    public List<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();

    public List<Message> Messages { get; set; } = new List<Message>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
