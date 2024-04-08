using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class CompetenceArea : AuditableEntity, IHasTenant, ISoftDelete
{
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; }

    public string Name { get; set; } = null!;

    public CompetenceArea? Parent { get; set; }

    public ICollection<CompetenceArea> Children { get; set; } = new List<CompetenceArea>();

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}