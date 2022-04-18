
using YourBrand.ApiKeys.Domain.Common;

namespace YourBrand.ApiKeys.Domain.Entities;

public class ApiKey : AuditableEntity, ISoftDelete, IHasDomainEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Key { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}