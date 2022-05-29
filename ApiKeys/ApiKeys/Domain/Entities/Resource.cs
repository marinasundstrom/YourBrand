using YourBrand.ApiKeys.Domain.Common;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Resource : AuditableEntity, ISoftDelete, IHasDomainEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public Service Service { get; set; }
    
    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}