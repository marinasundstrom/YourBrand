
using YourBrand.ApiKeys.Domain.Common;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Service : AuditableEntity, ISoftDelete, IHasDomainEvents
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string Secret { get; set; } = Guid.NewGuid().ToUrlFriendlyString();

    public List<Resource> Resources { get; set; } = new List<Resource>();
    
    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}