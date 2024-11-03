using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileIndustryExperiences : AuditableEntity<string>, IHasTenant, ISoftDeletableWithAudit
{
    public PersonProfileIndustryExperiences()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    //public OrganizationId OrganizationId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public Industry Industry { get; set; } = null!;

    public int IndustryId { get; set; }

    public int Years { get; set; }

    public bool IsDeleted { get; set; }

    public UserId? DeletedById { get; set; }

    public DateTimeOffset? Deleted { get; set; }
}