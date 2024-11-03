using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class CompetenceArea : AuditableEntity<string>, IHasTenant, ISoftDeletable
{
    public CompetenceArea()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; }

    public string Name { get; set; } = null!;

    public CompetenceArea? Parent { get; set; }

    public ICollection<CompetenceArea> Children { get; set; } = new List<CompetenceArea>();

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}