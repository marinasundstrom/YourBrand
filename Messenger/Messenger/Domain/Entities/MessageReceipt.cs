
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Entities;

public class MessageReceipt : AuditableEntity, IHasDomainEvents
{
    public string Id { get; set; } = null!;

    public Message Message { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
