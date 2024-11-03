using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileExperienceSkill : AuditableEntity<string>, IHasTenant, ISoftDeletable
{
    public PersonProfileExperienceSkill()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    //public OrganizationId OrganizationId { get; set; } = null!;

    public PersonProfileExperience PersonProfileExperience { get; set; } = null!;

    public PersonProfileSkill PersonProfileSkill { get; set; } = null!;

    public string PersonProfileSkillId { get; set; } = null!;

    public bool IsDeleted { get; set; } = false!;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}