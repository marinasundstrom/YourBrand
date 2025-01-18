using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileExperienceSkill : AuditableEntity<string>, IHasTenant, ISoftDeletableWithAudit<User>
{
    public PersonProfileExperienceSkill()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    //public OrganizationId OrganizationId { get; set; } = null!;

    public PersonProfileExperience? PersonProfileExperience { get; set; }

    public Employment? Employment { get; set; }

    public string? EmploymentId { get; set; }

    public Assignment? Assignment { get; set; }

    public string? AssignmentId { get; set; }

    public PersonProfileProject? Project { get; set; }

    public string? ProjectId { get; set; }

    public PersonProfileSkill PersonProfileSkill { get; set; } = null!;

    public string PersonProfileSkillId { get; set; } = null!;

    public bool IsDeleted { get; set; } = false!;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}